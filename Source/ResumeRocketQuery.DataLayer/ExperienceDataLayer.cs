using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Data;
using System.Security.Principal;

namespace ResumeRocketQuery.DataLayer
{
    public class ExperienceDataLayer : IExperienceDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public ExperienceDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertExperienceAsync(ExperienceStorage experience)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Experience.InsertExperience,
                    new
                    {
                        AccountId = experience.AccountId,
                        Company = experience.Company,
                        Position = experience.Position,
                        Type = experience.Type,
                        Description = experience.Description,
                        StartDate = experience.StartDate,
                        EndDate = experience.EndDate
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdateExperienceAsync(ExperienceStorage experience)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Experience.UpdateExperience,
                    new
                    {
                        ExperienceId = experience.ExperienceId,
                        AccountId = experience.AccountId,
                        Company = experience.Company,
                        Position = experience.Position,
                        Type = experience.Type,
                        Description = experience.Description,
                        StartDate = experience.StartDate,
                        EndDate = experience.EndDate
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<List<Experience>> GetExperienceAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<Experience>(
                    DataLayerConstants.StoredProcedures.Experience.SelectExperience,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

        public async Task DeleteExperienceAsync(int experienceId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Experience.DeleteExperience,
                    new { ExperienceId = experienceId },
                    commandType: CommandType.Text);
            }
        }
    }
}
