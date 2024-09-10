using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IAccountService
    {
        Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest createAccountRequest);
        Task<AccountDetails> GetAccountAsync(int accountId);
    }
}
