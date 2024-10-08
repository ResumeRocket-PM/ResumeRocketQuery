using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class SearchServiceTests
    {
        private readonly ISearchService _systemUnderTest;

        public SearchServiceTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<ISearchService>();
        }

        public class SearchAsync : SearchServiceTests
        {
            [Fact]
            public async Task WHEN_SearchAsync_is_called_THEN_results_returned()
            {
                var actual = await _systemUnderTest.SearchAsync("John", 10);

                Assert.Equal(10, actual.Count);
            }

        }
    }
}
