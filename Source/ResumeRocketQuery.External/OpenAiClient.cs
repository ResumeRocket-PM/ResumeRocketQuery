using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.External
{
    public interface IOpenAiClient
    {
        Task<string> SendMessageAsync(string prompt);
    }

    public class OpenAiClient : IOpenAiClient
    {

        /// <summary>
        /// First Use Case: Pass in a Prompt, and a Resume - Have it fine tune the resume based off the prompt.
        /// Second Use Case: Pass in all of the HTML - Have the Language Model parse it for us. It would return keywords based on that.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> SendMessageAsync(string prompt)
        {
            throw new NotImplementedException();
        }


    }
}
