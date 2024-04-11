using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public interface IOpenAiClient
    {
        Task<string> SendMessageAsync(string requirements, string prompt);
    }
}
