using ResumeRocketQuery.Domain.Services.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IApplicationDataLayer
    {
        Task<int> InsertApplicationAsync(ApplicationStorage application);
        Task UpdateApplicationAsync(ApplicationStorage application);
        Task<List<Application>> GetApplicationAsync(int accountId);
    }
}
