using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services;

public interface ILlamaClient
{
    Task<string> CreateMessage(string prompt, string input );
}