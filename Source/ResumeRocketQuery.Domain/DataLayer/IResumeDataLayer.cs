using ResumeRocketQuery.Domain.Services.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IResumeDataLayer
    {
        Task<int> InsertResumeAsync(ResumeStorage resume);
        Task UpdateResumeAsync(ResumeStorage resume);
        Task DeleteResumeAsync(int resumeId);
        Task<List<ResumeStorage>> GetResumeAsync(int accountId);
    }
}
