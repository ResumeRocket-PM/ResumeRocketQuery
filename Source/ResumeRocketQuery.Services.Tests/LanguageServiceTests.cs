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
            [Fact]
            public async Task WHEN_CaptureJobPostingAsync_is_called_THEN_account_is_created()
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

                var actual = await _systemUnderTest.CaptureJobPostingAsync("https://www.metacareers.com/jobs/624841835890795/");

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
