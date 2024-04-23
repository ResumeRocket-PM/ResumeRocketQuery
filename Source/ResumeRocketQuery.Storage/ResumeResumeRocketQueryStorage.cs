using Dapper;
using MySqlConnector;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
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

        public async Task<int> InsertResume(ResumeStorage resume)
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


        public async Task<List<ResumeStorage>> SelectResumeStorageAsync(int accoutnID)
        {
            using (var connection = new MySqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<ResumeStorage>(
                    StorageConstants.StoredProcedures.SelectResume,
                    new
                    {
                        accountID = accoutnID,
                    },
                    commandType: CommandType.Text);

                return result.ToList();

            }

        }
    }
    
    }
