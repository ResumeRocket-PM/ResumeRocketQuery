using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;

namespace ResumeRocketQuery.DataLayerIntegrationTests
{
    [Rollback]
    public class PortfolioDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public PortfolioDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private IPortfolioDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<IPortfolioDataLayer>();
        }

        [Theory]
        [InlineData(typeof(PortfolioDataLayer))]
        public async Task WHEN_InsertPortfolioAsync_is_called_THEN_portfolio_is_stored(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var portfolioId = await systemUnderTest.InsertPortfolioAsync(new PortfolioStorage
            {
                AccountId = 1,
                Configuration = "Sample Configuration"
            });

            Assert.True(portfolioId > 0);
        }

        [Theory]
        [InlineData(typeof(PortfolioDataLayer))]
        public async Task WHEN_InsertPortfolioAsync_is_called_THEN_storage_matches(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var expected = new PortfolioStorage
            {
                AccountId = 1,
                Configuration = "Sample Configuration"
            };

            var portfolioId = await systemUnderTest.InsertPortfolioAsync(expected);
            expected.PortfolioId = portfolioId;

            var actual = await systemUnderTest.GetPortfolioAsync(expected.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(PortfolioDataLayer))]
        public async Task WHEN_UpdatePortfolioAsync_is_called_THEN_portfolio_is_updated(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var portfolioId = await systemUnderTest.InsertPortfolioAsync(new PortfolioStorage
            {
                AccountId = 1,
                Configuration = "Sample Configuration"
            });

            var updatedPortfolio = new PortfolioStorage
            {
                PortfolioId = portfolioId,
                AccountId = 1,
                Configuration = "Updated Configuration",
            };

            await systemUnderTest.UpdatePortfolioAsync(updatedPortfolio);

            var actual = await systemUnderTest.GetPortfolioAsync(updatedPortfolio.AccountId);

            updatedPortfolio.ToExpectedObject().ShouldMatch(actual);
        }
    }


}
