using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IAccountService
    {
        Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest createAccountRequest);
        Task<Account> GetAccountAsync(int accountId);
    }
}
