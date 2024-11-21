using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IJobDataLayer
    {
        Task StoreJobPostingAsync(JobPosting posting);
        Task<JobPosting> GetJobPostingAsync(string url);
    }
}