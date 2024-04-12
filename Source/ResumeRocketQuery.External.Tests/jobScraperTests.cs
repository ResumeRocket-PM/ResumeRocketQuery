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
        private IjobScraper _systemUnderTest;

        public jobScraperTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IjobScraper>();
        }

        public class CreateAccountAsync : jobScraperTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_stored()
            {
                //jobScraper jbs = new();
                var result = await _systemUnderTest.scrapJobPosting("https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907");
                //string content = result;
                Console.WriteLine(result);
                Assert.NotNull(result);

                //try
                //{
                //    // Specify the file path on your desktop
                //    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //    string filePath = Path.Combine(desktopPath, "example.txt");

                //    // Write the string to the text file
                //    //string content = "Hello, world!";
                //    File.WriteAllText(filePath, result.ToString());

                //    Console.WriteLine("String has been written to the text file.");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($"An error occurred: {ex.Message}");
                //}

                //throw new Exception($"html {result.ToString()}");
                //await _systemUnderTest.scrapJobPosting("help me please");
            }
        }
    }
}
