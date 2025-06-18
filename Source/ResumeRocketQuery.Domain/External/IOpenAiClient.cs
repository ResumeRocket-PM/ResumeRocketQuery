using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public interface IOpenAiClient
    {
        Task<string> SendMessageAsync(string prompt, string message);
        IAsyncEnumerable<string> StreamMultiMessageAsync(int resumeId, int? applicationId, string prompt);
    }
}
