using ResumeRocketQuery.Domain.Services.Repository;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IAccountDataLayer
    {
        Task<int> InsertAccountStorageAsync(AccountStorage account);
        Task<Account> GetAccountAsync(int accountId);
        Task UpdateAccountStorageAsync(AccountStorage accountStorage);
    }
}
