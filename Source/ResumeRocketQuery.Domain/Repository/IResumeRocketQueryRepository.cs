using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Domain.Repository
{
    public interface IResumeRocketQueryRepository
    {
        Task<int> CreateAccountAsync(Account account);
        Task<Account> GetAccountAsync(int accountId);
        Task<Account> GetAccountByEmailAddressAsync(string emailAddress);
    }
}
