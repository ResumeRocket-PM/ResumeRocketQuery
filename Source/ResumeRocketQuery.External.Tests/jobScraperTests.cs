using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.External;
using System.IO;

namespace ResumeRocketQuery.Repository.Tests
{
    public class jobScraperTests
    {
        private IJobScraper _systemUnderTest;

        public jobScraperTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IJobScraper>();
        }

        public class CreateAccountAsync : jobScraperTests
        {
            [Theory]
            [InlineData("https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907")]
            [InlineData("https://openai.com/careers/endpoint-engineer")]
            [InlineData("https://www.metacareers.com/jobs/788246929742797/")]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_stored(string url)
            {
                var result = await _systemUnderTest.ScrapeJobPosting(url);

                Assert.NotNull(result);
            }
        }
    }
}
