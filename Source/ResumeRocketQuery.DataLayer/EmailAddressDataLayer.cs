using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Data;
using System.Security.Principal;

namespace ResumeRocketQuery.DataLayer
{
    public class EmailAddressDataLayer : IEmailAddressDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public EmailAddressDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertEmailAddressStorageAsync(EmailAddressStorage emailAddress)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.EmailAddress.InsertEmailAddress,
                    new
                    {
                        EmailAddress = emailAddress.EmailAddress,
                        AccountId = emailAddress.AccountId
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdateEmailAddressStorageAsync(EmailAddressStorage emailAddressStorage)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.EmailAddress.UpdateEmailAddress,
                    new
                    {
                        EmailAddress = emailAddressStorage.EmailAddress,
                        AccountId = emailAddressStorage.AccountId
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<EmailAddressRepository> GetEmailAddressAsync(int emailAddressId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<EmailAddressRepository>(
                    DataLayerConstants.StoredProcedures.EmailAddress.SelectAccountByEmailAddress,
                    new { EmailAddressId = emailAddressId },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<EmailAddressRepository> GetAccountByEmailAddressAsync(string emailAddress)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<EmailAddressRepository>(
                    DataLayerConstants.StoredProcedures.EmailAddress.SelectAccountByEmailAddress,
                    new { EmailAddress = emailAddress },
                    commandType: CommandType.Text);

                return result;
            }
        }
    }
}
