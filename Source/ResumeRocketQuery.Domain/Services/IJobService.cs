using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services;

public interface IJobService
{
    Task<int> CreateJobResumeAsync(Job job);
    Task<List<ApplicationResult>> GetJobPostings(int accountId);
    Task<ApplicationResult> GetResume(int resumeId);
    
    Task UpdateResume(int resumeId, string status);

}