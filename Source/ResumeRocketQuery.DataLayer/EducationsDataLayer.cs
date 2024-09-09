using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Data;
using System.Security.Principal;

namespace ResumeRocketQuery.DataLayer
{
    public class EducationsDataLayer : IEducationDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public EducationsDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertEducationStorageAsync(EducationStorage education)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Education.InsertEducation,
                    new
                    {
                        AccountId = education.AccountId,
                        SchoolName = education.SchoolName,
                        Degree = education.Degree,
                        Major = education.Major,
                        Minor = education.Minor,
                        GraduationDate = education.GraduationDate
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdateEducationStorageAsync(EducationStorage educationStorage)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Education.UpdateEducation,
                    new
                    {
                        EducationId = educationStorage.EducationId,
                        SchoolName = educationStorage.SchoolName,
                        Degree = educationStorage.Degree,
                        Major = educationStorage.Major,
                        Minor = educationStorage.Minor,
                        GraduationDate = educationStorage.GraduationDate
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task DeleteEducationStorageAsync(int educationId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Education.DeleteEducation,
                    new
                    {
                        EducationId = educationId
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<List<Education>> GetEducationAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<Education>(
                    DataLayerConstants.StoredProcedures.Education.SelectEducation,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }
    }
}
