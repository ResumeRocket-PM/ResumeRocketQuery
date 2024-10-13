using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services;

public interface IResumeService
{    Task<ApplicationResult> GetResume(int resumeId);
    Task<ApplicationResult> GetResumeHistory(int originalResumeId);
    Task UpdateResume(int resumeId, string status);
}