using System.IO;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services;

public interface ILlamaClient
{
    Task<string> CreateMessage(string prompt, string input);

    Task<string> JobDetails(Stream stream, string siteName);
    Task<string> ParseJobPosting(string html);
}