using System.Collections.Generic;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Domain.Repository
{
    public interface IResumeRocketQueryRepository
    {
        Task<int> CreateAccountAsync(Account account);
        Task<int> CreatePortfolioAsync(Portfolio portfolio);
        Task<int> CreateResumeAsync(Resume resume);
        Task<Account> GetAccountAsync(int accountId);
        Task<Account> GetAccountByEmailAddressAsync(string emailAddress);
        Task<Portfolio> GetPortfolioAsync(int accountId);
        Task<List<Resume>> GetResumesAsync(int accountId);
        Task<Resume> GetResumeAsync(int resumeId);

        Task UpdateResume(ResumeStorage resumeStorage);

    }
}
