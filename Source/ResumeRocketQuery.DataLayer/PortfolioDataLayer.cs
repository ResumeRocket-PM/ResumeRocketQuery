using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Data;
using System.Security.Principal;

namespace ResumeRocketQuery.DataLayer
{
    public class PortfolioDataLayer : IPortfolioDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public PortfolioDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        public async Task<int> InsertPortfolioAsync(PortfolioStorage portfolio)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Portfolio.InsertPortfolio,
                    new
                    {
                        AccountId = portfolio.AccountId,
                        Configuration = portfolio.Configuration
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        public async Task UpdatePortfolioAsync(PortfolioStorage portfolio)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                await connection.ExecuteAsync(
                    DataLayerConstants.StoredProcedures.Portfolio.UpdatePortfolio,
                    new
                    {
                        PortfolioId = portfolio.PortfolioId,
                        Configuration = portfolio.Configuration,
                    },
                    commandType: CommandType.Text);
            }
        }

        public async Task<Portfolio> GetPortfolioAsync(int accountId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<Portfolio>(
                    DataLayerConstants.StoredProcedures.Portfolio.SelectPortfolio,
                    new { AccountId = accountId },
                    commandType: CommandType.Text);

                return result;
            }
        }
    }
}
