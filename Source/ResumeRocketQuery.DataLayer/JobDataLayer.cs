using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using System.Data;
using static ResumeRocketQuery.DataLayer.DataLayerConstants.StoredProcedures;

namespace ResumeRocketQuery.DataLayer
{
    public class JobDataLayer : IJobDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public JobDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task StoreJobPostingAsync(JobPosting job)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                        DataLayerConstants.StoredProcedures.Jobs.InsertJob,
                        new
                        {
                            Url = job.JobUrl,
                            Company = job.JobCompany,
                            Description = job.JobDescription
                        },
                        commandType: CommandType.Text);
            }
        }

        public async Task<JobPosting> GetJobPostingAsync(string url)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<JobPosting>(
                        DataLayerConstants.StoredProcedures.Jobs.GetJob,
                        new
                        {
                            Url = url
                        },
                        commandType: CommandType.Text);

                return result;
            }
        }
    }
}