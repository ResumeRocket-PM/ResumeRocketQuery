using System.IO;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services;

public interface ILlamaClient
{
    Task<string> CreateMessage(string input);

    Task<string> GetJobPosting(Stream stream, string siteName);
    Task<string> ParseJobPosting(string html);
}