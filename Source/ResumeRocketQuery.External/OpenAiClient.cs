﻿using System;
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

namespace ResumeRocketQuery.External
{
    public class OpenAiClient : IOpenAiClient
    {
        private readonly ConcurrentDictionary<string, ChatHistory> _userChats = new ConcurrentDictionary<string, ChatHistory>();
        private readonly OpenAIChatCompletionService _chatService;
        private readonly IJobService _jobService;
        private readonly IResumeDataLayer _resumeDataLayer;
        private readonly IApplicationDataLayer _applicationDataLayer;

        public OpenAiClient(IJobService jobService, IResumeDataLayer resumeDataLayer, IApplicationDataLayer applicationDataLayer)
        {
            _chatService = new OpenAIChatCompletionService("gpt-4o", "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");
            _jobService = jobService;
            _resumeDataLayer = resumeDataLayer;
            _applicationDataLayer = applicationDataLayer;
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

        public async Task<string> SendMultiMessageAsync(int resumeId, int? applicationId, string prompt)
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
        public string GetResumeText(string html)
        {
            var extract = new Regex("<div class=\"c x[0-9] y[0-9] w[0-9] h[0-9]\"><div class=\"t m[0-9] x[0-9] h[0-9] y[0-9] ff[0-9] fs[0-9] fc[0-9] sc[0-9] ls[0-9] ws[0-9]\">(.*)</div></div>").Match(html).Groups[1].Value;
            var text = new Regex("<.*?>").Replace(extract, "");
            return text;
        }
    }
}
