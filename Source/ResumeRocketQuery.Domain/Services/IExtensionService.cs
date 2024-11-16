using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services;

public interface IExtensionService
{
    Task<bool> IsJobApplication(string html);
}