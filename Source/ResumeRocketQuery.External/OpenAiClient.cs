using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.Collections.Generic;
using ResumeRocketQuery.Domain.External;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Collections.Concurrent;

namespace ResumeRocketQuery.External
{
    public class OpenAiClient : IOpenAiClient
    {
        private readonly ConcurrentDictionary<string, ChatHistory> _userChats = new ConcurrentDictionary<string, ChatHistory>();
        private readonly OpenAIChatCompletionService _resumeService;

        public OpenAiClient()
        {
            _resumeService = new OpenAIChatCompletionService("gpt-4o", "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");
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

            if (!_userChats.TryGetValue(chatId, out chatHistory))
            {
                // Initialize chat history if it doesn't exist
                var resume = "This is where resume content would go";
                var job = "This is where job content would go";

                chatHistory = new ChatHistory(@$"
                    You are a helpful assistant that will help answer my questions related to the job and resume.
                    If no job is provided, you will assume I am looking for a job in general. Greet me with a hello.

                    Resume:
                    {resume}

                    Job:
                    {job}
                ");
                _userChats[chatId] = chatHistory;
            }

            chatHistory.AddUserMessage(prompt);
            var response = await _resumeService.GetChatMessageContentAsync(chatHistory, new OpenAIPromptExecutionSettings());
            chatHistory.AddAssistantMessage(response.ToString());
            return response.ToString();
        }
    }
}
