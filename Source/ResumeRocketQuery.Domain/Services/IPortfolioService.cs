using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IPortfolioService
    {
        Task<int> CreatePortfolio(Portfolio portfolio);
        Task<Portfolio> GetPortfolio(int accountId);
        Task<Portfolio> GetPortfolioByPortfolioId(int portfolioId);
    }
}
