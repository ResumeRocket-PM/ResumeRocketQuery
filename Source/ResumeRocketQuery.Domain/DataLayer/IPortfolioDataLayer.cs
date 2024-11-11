using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IPortfolioDataLayer
    {
        Task<Portfolio> GetPortfolioAsync(int accountId);
        Task<Portfolio> GetPortfolioByPortfolioIdAsync(int portfolioId);

        Task<int> InsertPortfolioAsync(PortfolioStorage portfolio);
        Task UpdatePortfolioAsync(PortfolioStorage portfolio);
    }
}
