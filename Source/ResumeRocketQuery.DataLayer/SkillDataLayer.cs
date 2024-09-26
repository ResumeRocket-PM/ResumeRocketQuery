using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Data;
using System.Security.Principal;

namespace ResumeRocketQuery.DataLayer
{
    public class SkillDataLayer : ISkillDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public SkillDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertSkillAsync(SkillStorage skill)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Skills.InsertSkill,
                    new
                    {
                        AccountId = skill.AccountId,
                        Description = skill.Description
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<List<SkillStorage>> GetSkillAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<SkillStorage>(
                    DataLayerConstants.StoredProcedures.Skills.SelectSkill,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

        public async Task DeleteSkillAsync(int skillId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Skills.DeleteSkill,
                    new { SkillId = skillId },
                    commandType: CommandType.Text);
            }
        }

        public async Task DeleteSkillByAccountIdAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Skills.DeleteSkillByAccountId,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);
            }
        }
    }
}
