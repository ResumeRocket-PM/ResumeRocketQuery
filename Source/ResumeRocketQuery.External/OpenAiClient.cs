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
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> SendMessageAsync(string prompt, string message)
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                "gpt-3.5-turbo", // OpenAI Model name
                "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"); // OpenAI API Key
            var kernel = builder.Build();
            var result = await kernel.InvokePromptAsync(prompt, new() { ["input"] = message});

        // Used for back and forth chat model, would need to be in a loop with calls to front-end
            /*var chatGPT = kernel.GetRequiredService<IChatCompletionService>();
            var chat = new ChatHistory(requirements);
            for loop here
            Console.WriteLine($"User: {prompt}");
            chat.AddUserMessage(prompt);
            var assistantReply = await chatGPT.GetChatMessageContentAsync(chat, new OpenAIPromptExecutionSettings());
            chat.AddAssistantMessage(assistantReply.Content);*/

            Debug.WriteLine(result);
            return result.ToString();
        }
    }
}
