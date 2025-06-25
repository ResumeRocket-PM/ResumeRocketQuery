using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using System.Data;

namespace ResumeRocketQuery.DataLayer
{
    public class SearchDataLayer : ISearchDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public SearchDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<List<SearchResult>> SearchAsync(string searchTerm, int resultCount)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<SearchResult>(
                    DataLayerConstants.StoredProcedures.Search.SearchFullTextIndex,
                    new { 
                        SearchTerm = searchTerm, 
                        ResultCount = resultCount
                    },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }

        public async Task<List<SearchResult>> GetAllUsersAsync()
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<SearchResult>(
                    DataLayerConstants.StoredProcedures.Search.GetAllUsers,
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }
    }
}
