using ResumeRocketQuery.Domain.Services.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IExperienceDataLayer
    {
        Task<int> InsertExperienceAsync(ExperienceStorage experience);
        Task UpdateExperienceAsync(ExperienceStorage experience);
        Task<List<Experience>> GetExperienceAsync(int accountId);
        Task DeleteExperienceAsync(int experienceId);
    }
}
