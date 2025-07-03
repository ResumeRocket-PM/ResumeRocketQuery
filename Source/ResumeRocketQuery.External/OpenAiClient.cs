using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.DataLayer;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Domain.Configuration;
using System.Text;

namespace ResumeRocketQuery.External
{
    public class OpenAiClient : IOpenAiClient
    {
        private readonly ConcurrentDictionary<string, ChatHistory> _userChats = new ConcurrentDictionary<string, ChatHistory>();
        private readonly OpenAIChatCompletionService _chatService;
        private readonly IJobService _jobService;
        private readonly IResumeDataLayer _resumeDataLayer;
        private readonly IApplicationDataLayer _applicationDataLayer;
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public OpenAiClient(IJobService jobService, IResumeDataLayer resumeDataLayer, IApplicationDataLayer applicationDataLayer, IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
            _chatService = new OpenAIChatCompletionService("gpt-3.5-turbo", _resumeRocketQueryConfigurationSettings.OpenAI_API_Key); // this used to be using gpt-4o
            _jobService = jobService;
            _resumeDataLayer = resumeDataLayer;
            _applicationDataLayer = applicationDataLayer;
        }

        public async Task<string> SendMessageAsync(string prompt, string message)
        {
            var builder = Kernel.CreateBuilder();
            var models = new List<string>{"gpt-3.5-turbo" }; // old models that we don't give access to: "gpt-4o", "gpt-4-turbo", "gpt-4", "gpt-4o-mini", 
            foreach (var model in models)
            {
                try
                {
                    builder.AddOpenAIChatCompletion(
                        model, // OpenAI Model name
                        _resumeRocketQueryConfigurationSettings.OpenAI_API_Key); // OpenAI API Key
                    var kernel = builder.Build();
                    var result = await kernel.InvokePromptAsync(prompt, new() { ["input"] = message });
                    return result.ToString();
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
            return "";
        }

        public async IAsyncEnumerable<string> StreamMultiMessageAsync(int resumeId, int? applicationId, string prompt)
        {
            string chatId = applicationId != null ? $"{resumeId}-{applicationId}" : $"{resumeId}";
            ChatHistory chatHistory;

            // Initialize chat history if it doesn't exist
            if (!_userChats.TryGetValue(chatId, out chatHistory))
            {
                // Get application, if application ID provided
                Task<Application> application = null;
                if (applicationId != null)
                {
                    int id = applicationId ?? 0;
                    application = _applicationDataLayer.GetApplicationAsync(id);
                }

                // Populate job, if resume associated with application, and resume
                string job = null;
                if (application != null)
                    job = _jobService.GetJobPostingAsync(application.Result.JobPostingUrl).Result.JobDescription;
                string resumeHtml = _resumeDataLayer.GetResumeAsync(resumeId).Result.Resume;
                var resume = GetResumeText(resumeHtml);

                // Initialize chat history with context
                chatHistory = new ChatHistory(@$"
                    You are a helpful assistant that will help answer my questions related to the job and resume.
                    If no job is provided, you will assume I am looking for a job in general.

                    Resume:
                    {resume}

                    Job:
                    {job}
                ");

                // Add chat history to dictionary for concurrent access
                _userChats[chatId] = chatHistory;
            }

            chatHistory.AddUserMessage(prompt);

            // vvv all in one way .vvv // 
            //var response = await _chatService.GetChatMessageContentAsync(chatHistory, new OpenAIPromptExecutionSettings());
            //chatHistory.AddAssistantMessage(response.ToString());
            //yield return response.ToString();

            // streaming way: 
            await foreach (var content in _chatService.GetStreamingChatMessageContentsAsync(chatHistory, new OpenAIPromptExecutionSettings()))
            {
                if (content != null)
                {
                    yield return content.Content;
                }
            }
        }
        public string GetResumeText(string html)
        {
            // VVV this way was only getting the first page VVV

            //var extract = new Regex("<div class=\"c x[0-9] y[0-9] w[0-9] h[0-9]\"><div class=\"t m[0-9] x[0-9] h[0-9] y[0-9] ff[0-9] fs[0-9] fc[0-9] sc[0-9] ls[0-9] ws[0-9]\">(.*)</div></div>").Match(html).Groups[1].Value;
            //var text = new Regex("<.*?>").Replace(extract, "");
            //return text;


            // Match all divs that contain resume text content
            var matches = Regex.Matches(
                html,
                "<div class=\"t m\\d+ x\\d+ h\\d+ y\\w+ ff\\d+ fs\\d+ fc\\d+ sc\\d+ ls\\d+ ws\\d+\">(.*?)</div>",
                RegexOptions.Singleline
            );

            var sb = new StringBuilder();

            foreach (Match match in matches)
            {
                string line = match.Groups[1].Value;

                // Remove any internal tags like <span> within the line
                line = Regex.Replace(line, "<.*?>", string.Empty);

                // Decode HTML entities if needed
                line = System.Net.WebUtility.HtmlDecode(line);

                sb.AppendLine(line.Trim());
            }

            return sb.ToString().Trim();
        }
    }
}
