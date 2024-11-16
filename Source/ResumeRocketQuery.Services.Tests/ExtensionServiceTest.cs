using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class ExtensionServiceTest
    {
        private readonly IExtensionService _systemUnderTest;

        public ExtensionServiceTest()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IExtensionService>();
        }

        public class IsJobApplication : ExtensionServiceTest
        {
            [Fact]
            public async Task GIVEN_valid_job_html_is_passed_THEN_result_is_correct()
            {
                var actual = await _systemUnderTest.IsJobApplication(File.ReadAllText("./Samples/DigitalMarketing-Posting.html"));

                Assert.True(actual);
            }

            [Fact]
            public async Task GIVEN_invalid_job_html_is_passed_THEN_result_is_correct()
            {
                var html = File.ReadAllText("./Samples/NotAJobPosting.html");

                var actual = await _systemUnderTest.IsJobApplication(html);

                Assert.False(actual);
            }
        }

        public class CreateHtmlQueryForEmbeddingButton : ExtensionServiceTest
        {
            [Fact]
            public async Task GIVEN_valid_job_html_is_passed_THEN_result_is_correct()
            {
                var actual = await _systemUnderTest.CreateHtmlQueryForEmbeddingButton(File.ReadAllText("./Samples/DigitalMarketing-Posting.html"));

                Assert.NotNull(actual);
            }
        }
    }
}
