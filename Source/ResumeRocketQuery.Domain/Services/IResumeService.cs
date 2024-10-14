using System.Collections.Generic;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.DataLayer;

namespace ResumeRocketQuery.Domain.Services;

public interface IResumeService
{
    Task<List<ResumeResult>> GetResumeHistory(int originalResumeId);
    Task<ResumeResult> GetResume(int resumeId);
    Task<bool> UpdateResume(ResumeStorage resume);
    Task<ResumeResult> CreateResume(ResumeStorage resume);
}