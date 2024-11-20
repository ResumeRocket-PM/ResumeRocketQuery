using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using System.Data;
using static ResumeRocketQuery.DataLayer.DataLayerConstants.StoredProcedures;

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
                if (!resume.OriginalResume)
                {
                    var resumeVersionCount = await connection.ExecuteScalarAsync<int>(
                        DataLayerConstants.StoredProcedures.Resume.GetNumResumeVersionsByAccount,
                        new { OriginalResumeId = resume.OriginalResumeID, AccountId = resume.AccountId },
                        commandType: CommandType.Text
                    );

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
                        Version = resume.Version,
                        InsertDate = resume.InsertDate,
                        UpdateDate = resume.UpdateDate,
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

        public async Task<List<ResumeStorage>> GetResumesAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeStorage>(
                    DataLayerConstants.StoredProcedures.Resume.SelectResumeByAccount,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

        public async Task<ResumeStorage> GetResumeAsync(int resumeId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<ResumeStorage>(
                    DataLayerConstants.StoredProcedures.Resume.SelectResume,
                    new { ResumeId = resumeId },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<List<ResumeStorage>> GetResumeHistoryAsync(int originalResumeId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeStorage>(
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


        public async Task<List<ResumeChangesStorage>> SelectResumeChangesAsync(int resumeId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeChangesStorage>(
                    DataLayerConstants.StoredProcedures.Resume.SelectResumeChanges,
                    new { ResumeId = resumeId },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

        public async Task<List<ResumeChangesStorage>> SelectResumeSuggestionsByApplicationId(int applicationId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeChangesStorage>(
                                       DataLayerConstants.StoredProcedures.Resume.SelectResumeSuggestionsByApplicationId,
                                                          new { ApplicationId = applicationId},
                                                                             commandType: CommandType.Text);

                return result.ToList();
            }
        }   

        public async Task<int> InsertResumeChangeAsync(ResumeChangesStorage resumeChangesStorage)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                return await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Resume.InsertResumeChanges,
                    new
                    {
                        ResumeId = resumeChangesStorage.ResumeId,
                        OriginalText = resumeChangesStorage.OriginalText,
                        ModifiedText = resumeChangesStorage.ModifiedText,
                        ExplanationString = resumeChangesStorage.ExplanationString,
                        Accepted = resumeChangesStorage.Accepted,
                        HtmlID = resumeChangesStorage.HtmlID,
                        ApplicationId = resumeChangesStorage.ApplicationId
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task UpdateResumeChangeAsync(ResumeChangesStorage resumeChangesStorage)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Resume.UpdateResumeChanges,
                    new
                    {
                        ResumeChangeId = resumeChangesStorage.ResumeChangeId,
                        Accepted = resumeChangesStorage.Accepted,
                    },
                    commandType: CommandType.Text);
            }
        }
    }
}
