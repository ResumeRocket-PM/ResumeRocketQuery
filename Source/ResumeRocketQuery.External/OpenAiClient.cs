using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using ResumeRocketQuery.Domain.External;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Collections.Concurrent;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.DataLayer;

namespace ResumeRocketQuery.External
{
    public class OpenAiClient : IOpenAiClient
    {
        private readonly ConcurrentDictionary<string, ChatHistory> _userChats = new ConcurrentDictionary<string, ChatHistory>();
        private readonly OpenAIChatCompletionService _chatService;
        private readonly IJobService _jobService;
        private readonly IResumeDataLayer _resumeDataLayer;
        private readonly IApplicationService _applicationService;

        public OpenAiClient(IJobService jobService, IResumeDataLayer resumeDataLayer, IApplicationService applicationService)
        {
            _chatService = new OpenAIChatCompletionService("gpt-4o", "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");
            _jobService = jobService;
            _resumeDataLayer = resumeDataLayer;
            _applicationService = applicationService;
        }

        public async Task<string> SendMessageAsync(string prompt, string message)
        {
            var builder = Kernel.CreateBuilder();
            var models = new List<string>{"gpt-4o", "gpt-4-turbo", "gpt-4", "gpt-4o-mini", "gpt-3.5-turbo" };
            foreach (var model in models)
            {
                try
                {
                    builder.AddOpenAIChatCompletion(
                        model, // OpenAI Model name
                        "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"); // OpenAI API Key
                    var kernel = builder.Build();
                    var result = await kernel.InvokePromptAsync(prompt, new() { ["input"] = message });
                    return result.ToString();
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
            return "";
        }

        public async Task<string> SendMultiMessageAsync(int accountId, int resumeId, int? applicationId, string prompt)
        {
            string chatId = applicationId != null ? $"{accountId}-{resumeId}-{applicationId}" : $"{accountId}-{resumeId}";
            ChatHistory chatHistory;

            // Initialize chat history if it doesn't exist
            if (!_userChats.TryGetValue(chatId, out chatHistory))
            {
                // Get application, if application ID provided
                Task<ApplicationResult> application = null;
                if (applicationId != null)
                {
                    int id = applicationId ?? 0;
                    application = _applicationService.GetApplication(id);
                }

                // Populate job, if resume associated with application, and resume
                string job = null;
                if (application != null)
                    job = _jobService.GetJobPostingAsync(application.Result.JobUrl).Result.ToString();
                var resume = _resumeDataLayer.GetResumeAsync(resumeId).Result.Resume;

                // Initialize chat history with context
                chatHistory = new ChatHistory(@$"
                    You are a helpful assistant that will help answer my questions related to the job and resume.
                    If no job is provided, you will assume I am looking for a job in general. Greet me with a hello.

                    Resume:
                    {resume}

                    Job:
                    {job}
                ");

                // Add chat history to dictionary for concurrent access
                _userChats[chatId] = chatHistory;
            }

            chatHistory.AddUserMessage(prompt);
            var response = await _chatService.GetChatMessageContentAsync(chatHistory, new OpenAIPromptExecutionSettings());
            chatHistory.AddAssistantMessage(response.ToString());
            return response.ToString();
        }
    }
}
