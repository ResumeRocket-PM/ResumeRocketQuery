using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Collections.Generic;

namespace ResumeRocketQuery.Services.Tests
{
    public class LanguageServiceTests
    {
        private readonly ILanguageService _systemUnderTest;

        public LanguageServiceTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<ILanguageService>();
        }

        public class CaptureJobPostingAsync : LanguageServiceTests
        {
            [Theory]
            [InlineData("https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907")]
            [InlineData("https://openai.com/careers/endpoint-engineer")]
            [InlineData("https://www.metacareers.com/jobs/788246929742797/")]
            public async Task WHEN_CaptureJobPostingAsync_is_called_THEN_account_is_created(string url)
            {
                var expected = new 
                {
                    CompanyName = Expect.Any<string>(),
                    Description = Expect.Any<string>(),
                    Keywords = Expect.Any<List<string>>(),
                    Perks = Expect.Any<List<string>>(),
                    Requirements = Expect.Any<List<string>>(),
                    Title = Expect.Any<string>()
                };

                var actual = await _systemUnderTest.CaptureJobPostingAsync("https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907");

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
