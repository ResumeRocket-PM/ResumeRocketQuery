using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioDataLayer _portfolioDataLayer;
        private readonly IAccountService _accountService;


        public PortfolioService(IPortfolioDataLayer portfolioDataLayer, IAccountService accountService)
        {
            _portfolioDataLayer = portfolioDataLayer;
            _accountService = accountService;
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

                string portfolioLink = $"https://resume-rocket.net/{result}/portfolio"; // for deployment
                //string portfolioLink = $"http://localhost:5174/{result}/portfolio"; // temporary


                Dictionary<string, string> updates = new Dictionary<string, string>
                {
                    { "PortfolioLink", portfolioLink }
                };

                await _accountService.UpdateAccount(portfolio.AccountId, updates);
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

        public async Task<Portfolio> GetPortfolioByPortfolioId(int portfolioId)
        {
            var result = await _portfolioDataLayer.GetPortfolioByPortfolioIdAsync(portfolioId);

            return result;
        }
    }
}
