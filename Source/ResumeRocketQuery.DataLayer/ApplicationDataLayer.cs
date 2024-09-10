using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Data;
using System.Security.Principal;

namespace ResumeRocketQuery.DataLayer
{
    public class ApplicationDataLayer : IApplicationDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public ApplicationDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertApplicationAsync(ApplicationStorage application)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Applications.InsertApplication,
                    new
                    {
                        AccountId = application.AccountId,
                        ApplyDate = application.ApplyDate,
                        Status = application.Status,
                        Position = application.Position,
                        CompanyName = application.CompanyName
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdateApplicationAsync(ApplicationStorage application)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Applications.UpdateApplication,
                    new
                    {
                        ApplicationId = application.ApplicationId,
                        Status = application.Status,
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<List<Application>> GetApplicationAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<Application>(
                    DataLayerConstants.StoredProcedures.Applications.SelectApplicationByAccount,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

    }
}
