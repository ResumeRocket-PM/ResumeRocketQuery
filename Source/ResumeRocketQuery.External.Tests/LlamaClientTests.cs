using System;
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
    }
}
