using System.Collections.Generic;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IAccountDataLayer
    {
        Task<int> InsertAccountStorageAsync(AccountStorage account);
        Task<Account> GetAccountAsync(int accountId);
        Task UpdateAccountStorageAsync(AccountStorage accountStorage);
        Task<List<Account>> SelectAccountStoragesByFilterAsync(string filterType, string searchField);
    }
}
