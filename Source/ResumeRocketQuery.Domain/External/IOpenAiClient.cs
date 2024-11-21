using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public interface IOpenAiClient
    {
        Task<string> SendMessageAsync(string prompt, string message);
        Task<string> SendMultiMessageAsync(int resumeId, int? applicationId, string prompt);
    }
}
