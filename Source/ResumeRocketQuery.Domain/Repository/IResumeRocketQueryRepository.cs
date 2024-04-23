using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Domain.Repository
{
    public interface IResumeRocketQueryRepository
    {
        Task<int> CreateAccountAsync(Account account);
        Task<int> CreatePortfolioAsync(Portfolio portfolio);
        Task<Account> GetAccountAsync(int accountId);
        Task<Account> GetAccountByEmailAddressAsync(string emailAddress);
        Task<Portfolio> GetPortfolioAsync(int accountId);
    }
}
