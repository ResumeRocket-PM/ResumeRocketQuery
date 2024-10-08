using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;

namespace ResumeRocketQuery.DataLayerIntegrationTests
{
    [Rollback]
    public class SearchDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public SearchDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
        }

        private ISearchDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<ISearchDataLayer>();
        }

        [Theory]
        [InlineData(typeof(SkillDataLayer))]
        public async Task WHEN_SearchAsync_is_called_THEN_results_returned_correctly(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchAsync("John", 10);

            Assert.Equal(10, results.Count);
        }
    }
}
