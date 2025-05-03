using System.Threading.Tasks;
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
            [InlineData("https://www.metacareers.com/jobs/1408007706638053/")]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_stored(string url)
            {
                _systemUnderTest.ScrapeSetup(url);
                var result = await _systemUnderTest.ScrapeJobPosting("//html");
                Assert.NotNull(result);


                //var result = await _systemUnderTest.ScrapeJobPosting(url);
                //Assert.NotNull(result);
            }
            [Theory]
            [InlineData("https://www.metacareers.com/jobs/1408007706638053/")]
            public async Task TestScrapeSavedFile(string url)
            {
                _systemUnderTest.ScrapeSetup(url);
                var result = await _systemUnderTest.SaveHtmlFile("testpage");
                Assert.True(result);
            }
            [Theory]
            [InlineData("https://www.metacareers.com/resume/?req=a1K2K000008UcBvUAK")]
            public async Task TestFindTextInputField(string url)
            {
                _systemUnderTest.ScrapeSetup(url);
                var inputFields = await _systemUnderTest.TextInputFieldNames();
                Assert.True(inputFields.Count != 0);
            }

            [Theory]
            [InlineData("https://www.metacareers.com/resume/?req=a1K2K000008UcBvUAK")]
            public async Task TestFindCheckBox(string url)
            {
                _systemUnderTest.ScrapeSetup(url);
                var inputFields = await _systemUnderTest.CheckBoxInputFieldNames();

                Assert.True(inputFields.Count != 0);
            }

            //[Theory]
            //[InlineData("https://www.metacareers.com/resume/?req=a1K2K000007p93VUAQ")]
            //public async Task TestAutoFilledCheckBox(string url)
            //{
            //    _systemUnderTest.ScrapeSetup(url);
            //    var inputFields = await _systemUnderTest.CheckBoxInputFieldNames();

            //    Dictionary<string, string> autoFilledDict = new Dictionary<string, string>();
            //    autoFilledDict["cb"] = inputFields[(inputFields.Count-1)];

            //    bool filledSuccess = await _systemUnderTest.submitFilledForm(autoFilledDict);
            //    Assert.True(filledSuccess);
            //}

            [Theory]
            [InlineData("https://www.metacareers.com/resume/?req=a1K2K000008UcBvUAK")]
            public async Task TestAutoFilledTextBox(string url)
            {
                _systemUnderTest.ScrapeSetup(url);
                var inputFields = await _systemUnderTest.TextInputFieldNames();
                //var CbFields = await _systemUnderTest.CheckBoxInputFieldNames();

                Dictionary<string, string> autoFilledDict = new Dictionary<string, string>();

                //autoFilledDict[CbFields[(CbFields.Count - 1)]] ="cb" ;

                for (int i = 0; i < inputFields.Count; i++)
                {
                    autoFilledDict[inputFields[i]] = $"RR Unit Test {i}";
                }

                bool filledSuccess = await _systemUnderTest.submitFilledForm(autoFilledDict);
                Assert.True(filledSuccess);
            }

        }
    }
}
