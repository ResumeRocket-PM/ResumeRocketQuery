using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface ILanguageService
    {
        Task<JobResult> CaptureJobPostingAsync(string url);
        Task<JobResult> ProcessJobPosting(string html, string url);
    }
}
