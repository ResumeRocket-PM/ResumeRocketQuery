using ResumeRocketQuery.Domain.Services.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface ISkillDataLayer
    {
        Task<int> InsertSkillAsync(SkillStorage skill);
        Task<List<SkillStorage>> GetSkillAsync(int accountId);
        Task DeleteSkillAsync(int skillId);
    }
}
