using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IPortfolioService
    {
        Task CreatePortfolio(Portfolio portfolio);
        Task<Portfolio> GetPortfolio(int accountId);
    }
}
