using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;

namespace ResumeRocketQuery.External.Tests
{
    public class LlamaClientTests
    {
        private ILlamaClient _systemUnderTest;

        public LlamaClientTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<ILlamaClient>();
        }

        public class ConvertPdf : LlamaClientTests
        {
            [Fact]
            public async Task WHEN_Convert_is_called_THEN_response_is_NOT_NULL()
            {
                var result = await _systemUnderTest.CreateMessage("Hello, can you tell me what a json object is?");

                Assert.NotNull(result);
            }
        }

        public class GetJobPosting : LlamaClientTests
        {
            [Fact]
            public async Task WHEN_GetJobPosting_is_called_THEN_response_is_NOT_NULL()
            {
                using (var stream = new FileStream(@"./Samples/Meta/meta-swe.html", FileMode.Open, FileAccess.Read))
                {
                    var result = await _systemUnderTest.GetJobPosting(stream, "meta");
                    Assert.NotNull(result);
                }
                 using (var stream = new FileStream(@"./Samples/Indeed/indeed-swe.html", FileMode.Open, FileAccess.Read))
                {
                    var result = await _systemUnderTest.GetJobPosting(stream, "indeed");
                    Assert.NotNull(result);
                }
                 using (var stream = new FileStream(@"./Samples/LinkedIn/linkedin-de.html", FileMode.Open, FileAccess.Read))
                {
                    var result = await _systemUnderTest.GetJobPosting(stream, "linkedin");
                    Assert.NotNull(result);
                }
            }
        }

        public class ParseJobPosting : LlamaClientTests
        {
            [Fact]
            public async Task WHEN_ParseJobPosting_is_called_THEN_response_is_NOT_NULL()
            {
                using (var stream = new FileStream(@"./Samples/Meta/meta-swe.html", FileMode.Open, FileAccess.Read))
                {
                    var result = await _systemUnderTest.GetJobPosting(stream, "meta");
                    result = await _systemUnderTest.ParseJobPosting(result);
                    Assert.NotNull(result);
                }
            }
        }
    }
}
