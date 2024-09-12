using ResumeRocketQuery.Domain.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ResumeRocketQuery.External
{
    public class OpenAiClient : IOpenAiClient
    {
        /// <summary>
        /// First Use Case: Pass in a Prompt, and a Resume - Have it fine tune the resume based off the prompt.
        /// Second Use Case: Pass in all of the HTML - Have the Language Model parse it for us. It would return keywords based on that.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>Response message from ChatGPT</returns>
        public async Task<string> SendMessageAsync(string prompt, string message)
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                "gpt-4o-mini", // OpenAI Model name
                "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"); // OpenAI API Key
            var kernel = builder.Build();
            var result = await kernel.InvokePromptAsync(prompt, new() { ["input"] = message});            

            return result.ToString();
        }

        public async Task<string> SendMultiMessageAsync(List<string> prompts)
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                "gpt-4o-mini", // OpenAI Model name
                "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"); // OpenAI API Key
            var kernel = builder.Build();
            var chatGPT = kernel.GetRequiredService<IChatCompletionService>();
            var chat = new ChatHistory();
            var response = "";
            foreach (var prompt in prompts)
            {
                chat.AddUserMessage(prompt);
                var assistantReply = await chatGPT.GetChatMessageContentAsync(chat, new OpenAIPromptExecutionSettings());
                response = assistantReply.Content;
                chat.AddAssistantMessage(response);
            }
            return response;
        }
    }
}
