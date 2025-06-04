using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Data;

namespace ResumeRocketQuery.DataLayer
{
    public class AccountDataLayer : IAccountDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public AccountDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertAccountStorageAsync(AccountStorage account)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Account.InsertAccount,
                    new
                    {
                        accountAlias = account.AccountAlias,
                        firstName = account.FirstName,
                        lastName = account.LastName,
                        profilePhotoLink = account.ProfilePhotoLink,
                        title = account.Title,
                        stateLocation = account.StateLocation,
                        portfolioLink = account.PortfolioLink,
                        primaryResumeId = account.PrimaryResumeId
                    },
                    commandType: CommandType.Text);
                return result;
            }
        }

        public async Task<Account> GetAccountAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<Account>(
                    DataLayerConstants.StoredProcedures.Account.SelectAccount,
                    new
                    {
                        accountID = accountId,
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdateAccountStorageAsync(AccountStorage accountStorage)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Account.UpdateAccount,
                    new
                    {
                        accountId = accountStorage.AccountId,
                        accountAlias = accountStorage.AccountAlias,
                        firstName = accountStorage.FirstName,
                        lastName = accountStorage.LastName,
                        profilePhotoLink = accountStorage.ProfilePhotoLink,
                        backgroundPhotoLink = accountStorage.BackgroundPhotoLink,
                        title = accountStorage.Title,
                        stateLocation = accountStorage.StateLocation,
                        portfolioLink = accountStorage.PortfolioLink,
                        PrimaryResumeId = accountStorage.PrimaryResumeId
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<List<Account>> SelectAccountStoragesByFilterAsync(string filterType, string searchField)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<Account>(
                    DataLayerConstants.StoredProcedures.Account.SelectAccountByFilter,
                    new
                    {
                        SearchField = filterType,
                        SearchParameter = searchField
                    },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }
    }
}
