using System.Collections.Generic;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IAccountService
    {
        Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest createAccountRequest);
        Task CreateEducationsAsync(int accountId, List<Education> educations);
        Task CreateExperiencesAsync(int accountId, List<Experience> experiences);
        Task CreateSkillsAsync(int accountId, List<Skill> skills);
        Task<AccountDetails> GetAccountAsync(int accountId);
        Task UpdateAccount(int accountId, Dictionary<string, string> updates);
        Task UpdateAccount(AccountDetails accountDetails);
    }
}
