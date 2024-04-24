using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Storage
{
    public class DapperResumeRocketQueryStorage : IResumeRocketQueryStorage
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public DapperResumeRocketQueryStorage(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertPortfolioStorageAsync(PortfolioStorage portfolio)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    StorageConstants.StoredProcedures.InsertPortfolio,
                    new
                    {
                        AccountId = portfolio.AccountId,
                        PortfolioAlias = portfolio.PortfolioAlias,
                        PortfolioConfiguration = portfolio.PortfolioConfiguration
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<PortfolioStorage> SelectPortfolioStorageAsync(int accountId)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<PortfolioStorage>(
                    StorageConstants.StoredProcedures.SelectPortfolio,
                    new
                    {
                        AccountID = accountId
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }


        public async Task<int> InsertAccountStorageAsync(AccountStorage accountStorage)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    StorageConstants.StoredProcedures.InsertAccount,
                    new
                    {
                        AccountAlias = accountStorage.AccountAlias,
                        AccountConfiguration = accountStorage.AccountConfiguration
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<AccountStorage> SelectAccountStorageAsync(int accountId)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<AccountStorage>(
                    StorageConstants.StoredProcedures.SelectAccount,
                    new
                    {
                        AccountID = accountId
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<int> InsertEmailAddressStorageAsync(EmailAddressStorage emailAddressStorage)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    StorageConstants.StoredProcedures.InsertEmailAddress,
                    new
                    {
                        EmailAddress = emailAddressStorage.EmailAddress,
                        AccountID = emailAddressStorage.AccountId
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageByEmailAddressAsync(string emailAddress)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<EmailAddressStorage>(
                    StorageConstants.StoredProcedures.SelectEmailAddressByEmailAddress,
                    new
                    {
                        EmailAddress = emailAddress
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageAsync(int emailAddressId)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<EmailAddressStorage>(
                    StorageConstants.StoredProcedures.SelectEmailAddress,
                    new
                    {
                        EmailAddressID = emailAddressId
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageByAccountIdAsync(int accountId)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<EmailAddressStorage>(
                    StorageConstants.StoredProcedures.SelectEmailAddressByAccountId,
                    new
                    {
                        AccountID = accountId
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<int> InsertResumeStorageAsync(ResumeStorage resume)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    StorageConstants.StoredProcedures.InsertResume,
                    new
                    {
                        applyDate = resume.applyDate,
                        jobUrl = resume.jobUrl,
                        accountID = resume.accountID,
                        status = resume.status,
                        resume = resume.resume,
                        position = resume.position,
                        companyName = resume.companyName,
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<List<ResumeStorage>> SelectResumeStoragesAsync(int accountId)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeStorage>(
                    StorageConstants.StoredProcedures.SelectResumes,
                    new
                    {
                        accountID = accountId,
                    },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

        public async Task<ResumeStorage> SelectResumeStorageAsync(int resumeId)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<ResumeStorage>(
                    StorageConstants.StoredProcedures.SelectResume,
                    new
                    {
                        ResumeID = resumeId,
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdateResumeStorageAsync(ResumeStorage resumeStorage)
        {
            //Given the ResumeId on the ResumeStorage Object,
            //Update the record with that Id, and set the status to whatever the status of this object is.
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    StorageConstants.StoredProcedures.changeStatus,
                new
                {
                    resumeID = resumeStorage.ResumeID,
                    status = resumeStorage.status,
                },
                commandType: CommandType.Text);

                //var changeStatus = await connection.query

            }
        }
    }
}
