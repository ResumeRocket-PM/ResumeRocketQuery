using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Data;
using System.Security.Principal;

namespace ResumeRocketQuery.DataLayer
{
    public class LoginDataLayer : ILoginDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public LoginDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertLoginStorageAsync(LoginStorage login)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Logins.InsertLogin,
                    new
                    {
                        AccountId = login.AccountId,
                        Salt = login.Salt,
                        Hash = login.Hash
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdateLoginStorageAsync(LoginStorage loginStorage)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Logins.UpdateLogin,
                    new
                    {
                        AccountId = loginStorage.AccountId,
                        Salt = loginStorage.Salt,
                        Hash = loginStorage.Hash
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<Login> GetLoginAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<Login>(
                    DataLayerConstants.StoredProcedures.Logins.SelectLogin,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result;
            }
        }
    }
}
