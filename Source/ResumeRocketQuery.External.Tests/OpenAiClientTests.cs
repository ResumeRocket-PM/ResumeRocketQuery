using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ResumeRocketQuery.Domain.External;

namespace ResumeRocketQuery.Repository.Tests
{
    public class OpenAiClientTests
    {
        private IOpenAiClient _systemUnderTest;

        public OpenAiClientTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IOpenAiClient>();
        }

        public class CreateAccountAsync : OpenAiClientTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_stored()
            {
                await _systemUnderTest.SendMessageAsync("help me please");
            }
        }
    }
}
