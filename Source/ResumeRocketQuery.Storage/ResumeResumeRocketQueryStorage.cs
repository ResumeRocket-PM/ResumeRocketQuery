using Dapper;
using MySqlConnector;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Storage
{
    public class ResumeResumeRocketQueryStorage : IResumeQueryStorage
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public ResumeResumeRocketQueryStorage(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<string> InsertResume( string url, string resume)
        {
            ResumeStorage newResume = new();
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                // ExecuteScalarAsync??
                var result = await connection.ExecuteScalarAsync<string>(
                    StorageConstants.StoredProcedures.InsertResume,
                    new
                    {
                        AccountID = newResume.AccountId,
                        JobURL = url,
                        Resume = resume
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task<string> SelectResumeStorageAsync(string Url, int accountID)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<string>(
                    StorageConstants.StoredProcedures.SelectResume,
                    new
                    {
                        JobUrl = Url,
                        AccountID = accountID
                    },
                    commandType: CommandType.Text) ;

                return result;

            }

        }
    }
    
    }
