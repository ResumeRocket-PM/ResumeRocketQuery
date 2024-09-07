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
using iText.Layout.Element;
using System.Collections.Generic;

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
            //[InlineData("https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907")]
            //[InlineData("https://openai.com/careers/endpoint-engineer")]
            [InlineData("https://www.metacareers.com/jobs/788246929742797/")]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_stored(string url)
            {
                _systemUnderTest.ScrapeSetup(url);
                var result = await _systemUnderTest.ScrapeJobPosting("//html");
                Assert.NotNull(result);


                //var result = await _systemUnderTest.ScrapeJobPosting(url);
                //Assert.NotNull(result);
            }

            [Theory]
            [InlineData("https://www.metacareers.com/resume/?req=a1K2K000007p93VUAQ")]
            public async Task TestAllInputField(string url)
            {
                _systemUnderTest.ScrapeSetup(url);
                var inputFields = await _systemUnderTest.InputFieldNames();
                Assert.True(inputFields.Count != 0);


            }

            [Theory]
            [InlineData("https://www.metacareers.com/resume/?req=a1K2K000007p93VUAQ")]
            public async Task TestAutoFilledApplication(string url)
            {
                _systemUnderTest.ScrapeSetup(url);
                var inputFields = await _systemUnderTest.InputFieldNames();

                Dictionary<string, string> autoFilledDict = new Dictionary<string, string>();
                for(int i = 0; i < inputFields.Count; i++)
                {
                    bool isNum = int.TryParse(inputFields[i], out var num);
                    if (!isNum && inputFields[i]!="")
                    {
                        autoFilledDict[inputFields[i]] = $"RR Unit Test {i}";
                    }
                }

                var obj = await _systemUnderTest.submitFilledForm(autoFilledDict);
                Assert.NotNull(obj);



            }
        }
    }
}
