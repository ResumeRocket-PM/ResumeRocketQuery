using System.Threading.Tasks;
using ResumeRocketQuery.Domain.DataLayer;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IJobService
    {
        // Method handles
        Task StoreJobPostingAsync(string url, string company, string posting);
        Task<JobPosting> GetJobPostingAsync(string url);
    }
}