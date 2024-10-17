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
                // If OriginalResume is true, find out how many original resumes exist for the account.
                if (resume.OriginalResume == true)
                {
                    var originalResumeCount = await connection.ExecuteScalarAsync<int>(
                        DataLayerConstants.StoredProcedures.Resume.GetNumOriginalResumesByAccount,
                        new { AccountId = resume.AccountId }, // Pass the AccountId to the query
                        commandType: CommandType.Text
                    );

                    // Set OriginalResumeID to originalResumeCount + 1
                    resume.OriginalResumeID = originalResumeCount + 1;
                }
                else
                {
                    // If OriginalResume is false, get the number of resume versions for the OriginalResumeId.
                    var resumeVersionCount = await connection.ExecuteScalarAsync<int>(
                        DataLayerConstants.StoredProcedures.Resume.GetNumResumeVersionsByAccount,
                        new { OriginalResumeId = resume.OriginalResumeID, AccountId = resume.AccountId }, // Pass OriginalResumeId and AccountId
                        commandType: CommandType.Text
                    );

                    // Set the Version to the number of versions + 1
                    resume.Version = resumeVersionCount + 1;
                }

                // insert the resume
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

        public async Task<List<ResumeResult>> GetResumeHistoryAsync(int originalResumeId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeResult>(
                    DataLayerConstants.StoredProcedures.Resume.SelectResumeByOriginal,
                    new { originalResumeId = originalResumeId },
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

        //public async Task GetNumResumeVersions(int originalResumeId)
        //{
        //    using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
        //    {
        //        await connection.ExecuteAsync(
        //            DataLayerConstants.StoredProcedures.Resume.GetNumResumeVersions,
        //            new { OriginalResumeID = originalResumeId },
        //            commandType: CommandType.Text);
        //    }
        //}
    }
}
