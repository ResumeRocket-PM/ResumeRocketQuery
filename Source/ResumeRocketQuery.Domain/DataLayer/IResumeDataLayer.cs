using ResumeRocketQuery.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IResumeDataLayer
    {
        Task<int> InsertResumeAsync(ResumeStorage resume);
        Task UpdateResumeAsync(ResumeStorage resume);
        Task DeleteResumeAsync(int resumeId);
        Task<List<ResumeStorage>> GetResumesAsync(int accountId);
        Task<ResumeStorage> GetResumeAsync(int resumeId);
        Task<List<ResumeStorage>> GetResumeHistoryAsync(int originalResumeId);
    }
}
