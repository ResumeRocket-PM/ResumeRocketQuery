using ResumeRocketQuery.Domain.Services.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IEducationDataLayer
    {
        Task<int> InsertEducationStorageAsync(EducationStorage education);
        Task UpdateEducationStorageAsync(EducationStorage educationStorage);
        Task DeleteEducationStorageAsync(int educationId);
        Task<List<Education>> GetEducationAsync(int accountId);
        Task DeleteEducationStorageByAccountAsync(int accountId);
    }
}
