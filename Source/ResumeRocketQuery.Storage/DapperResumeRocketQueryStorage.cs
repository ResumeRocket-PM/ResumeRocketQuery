using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;

namespace ResumeRocketQuery.Storage
{
    public class DapperResumeRocketQueryStorage : IResumeRocketQueryStorage
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public DapperResumeRocketQueryStorage(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertAccountStorageAsync(AccountStorage accountStorage)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    StorageConstants.StoredProcedures.InsertAccount,
                    new
                    {
                        AccountAlias = accountStorage.AccountAlias,
                        AccountConfiguration = accountStorage.AccountConfiguration
                    },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<AccountStorage> SelectAccountStorageAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<AccountStorage>(
                    StorageConstants.StoredProcedures.SelectAccount,
                    new
                    {
                        AccountID = accountId
                    },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<int> InsertEmailAddressStorageAsync(EmailAddressStorage emailAddressStorage)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    StorageConstants.StoredProcedures.InsertEmailAddress,
                    new
                    {
                        EmailAddress = emailAddressStorage.EmailAddress,
                        AccountID = emailAddressStorage.AccountId
                    },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageByEmailAddressAsync(string emailAddress)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<EmailAddressStorage>(
                    StorageConstants.StoredProcedures.SelectEmailAddressByEmailAddress,
                    new
                    {
                        EmailAddress = emailAddress
                    },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageAsync(int emailAddressId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<EmailAddressStorage>(
                    StorageConstants.StoredProcedures.SelectEmailAddress,
                    new
                    {
                        EmailAddressID = emailAddressId
                    },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageByAccountIdAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<EmailAddressStorage>(
                    StorageConstants.StoredProcedures.SelectEmailAddressByAccountId,
                    new
                    {
                        AccountID = accountId
                    },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }
    }
}
