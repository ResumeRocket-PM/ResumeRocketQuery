using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface ISkillDataLayer
    {
        Task<int> InsertSkillAsync(SkillStorage skill);
        Task<List<SkillStorage>> GetSkillAsync(int accountId);
        Task DeleteSkillAsync(int skillId);
        Task DeleteSkillByAccountIdAsync(int accountId);
    }
}
