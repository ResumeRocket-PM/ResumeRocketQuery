using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IResumeRocketQueryRepository _resumeRocketQueryRepository;

        public PortfolioService(IResumeRocketQueryRepository resumeRocketQueryRepository)
        {
            _resumeRocketQueryRepository = resumeRocketQueryRepository;
        }

        public async Task CreatePortfolio(Portfolio portfolio)
        {
            await _resumeRocketQueryRepository.CreatePortfolioAsync(portfolio);
        }

        public async Task<Portfolio> GetPortfolio(int accountId)
        {
            var result = await _resumeRocketQueryRepository.GetPortfolioAsync(accountId);

            return result;
        }
    }
}
