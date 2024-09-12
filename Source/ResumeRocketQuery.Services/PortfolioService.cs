using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioDataLayer _portfolioDataLayer;

        public PortfolioService(IPortfolioDataLayer portfolioDataLayer)
        {
            _portfolioDataLayer = portfolioDataLayer;
        }

        public async Task<int> CreatePortfolio(Portfolio portfolio)
        {
            var existing = await GetPortfolio(portfolio.AccountId);

            var result = existing?.PortfolioId ?? 0;

            if (existing == null)
            {
                result = await _portfolioDataLayer.InsertPortfolioAsync(new PortfolioStorage
                {
                    AccountId = portfolio.AccountId,
                    Configuration = portfolio.Configuration,
                });
            }
            else
            {
                await _portfolioDataLayer.UpdatePortfolioAsync(new PortfolioStorage
                {
                    PortfolioId = existing.PortfolioId,
                    AccountId = portfolio.AccountId,
                    Configuration = portfolio.Configuration,
                });
            }

            return result;
        }

        public async Task<Portfolio> GetPortfolio(int accountId)
        {
            var result = await _portfolioDataLayer.GetPortfolioAsync(accountId);

            return result;
        }
    }
}
