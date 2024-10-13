using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using System.Data;

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
                        Resume = resume.Resume,
                        OriginalResumeID = resume.OriginalResumeID,
                        OriginalResume = resume.OriginalResume,
                        Version = resume.Version
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
                        Resume = resume.Resume,
                        OriginalResumeID = resume.OriginalResumeID,
                        OriginalResume = resume.OriginalResume,
                        Version = resume.Version
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<List<ResumeResult>> GetResumesAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeResult>(
                    DataLayerConstants.StoredProcedures.Resume.SelectResumeByAccount,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

        public async Task<ResumeResult> GetResumeAsync(int resumeId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<ResumeResult>(
                    DataLayerConstants.StoredProcedures.Resume.SelectResume,
                    new { ResumeId = resumeId },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<ResumeResult> GetResumeHistoryAsync(int originalResumeId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<ResumeResult>(
                    DataLayerConstants.StoredProcedures.Resume.SelectResume,
                    new { originalResumeId = originalResumeId },
                    commandType: CommandType.Text);

                return result;
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
