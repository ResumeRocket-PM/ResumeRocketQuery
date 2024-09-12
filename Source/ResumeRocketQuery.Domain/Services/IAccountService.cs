using System.Collections.Generic;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IAccountService
    {
        Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest createAccountRequest);
        Task CreateEducation(Education education);
        Task CreateExperience(Experience experience);
        Task CreateSkill(Skill education, int accountId);
        Task<AccountDetails> GetAccountAsync(int accountId);
        Task UpdateAccount(int accountId, Dictionary<string, string> updates);
    }
}
