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
    public class ResumeDataLayer : IResumeDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public ResumeDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertResumeAsync(ResumeStorage resume)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Resume.InsertResume,
                    new
                    {
                        AccountId = resume.AccountId,
                        Resume = resume.Resume
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdateResumeAsync(ResumeStorage resume)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Resume.UpdateResume,
                    new
                    {
                        ResumeId = resume.ResumeId,
                        Resume = resume.Resume
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<List<ResumeStorage>> GetResumeAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeStorage>(
                    DataLayerConstants.StoredProcedures.Resume.SelectResume,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

        public async Task DeleteResumeAsync(int resumeId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Resume.DeleteResume,
                    new { ResumeId = resumeId },
                    commandType: CommandType.Text);
            }
        }
    }
}
