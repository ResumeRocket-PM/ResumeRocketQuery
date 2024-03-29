using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class AccountServiceTests
    {
        private readonly IAccountService _systemUnderTest;

        public AccountServiceTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IAccountService>();
        }

        public class CreateAccountAsync : AccountServiceTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_created()
            {
                var expected = new
                {
                    AccountId = Expect.NotDefault<int>()
                };

                var actual = await _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = @"{Guid.NewGuid().ToString()}@gmail.com",
                    Password = Guid.NewGuid().ToString()
                });

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_matches_response()
            {
                var emailAddress = @"{Guid.NewGuid().ToString()}@gmail.com";

                var createResponse = await _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = Guid.NewGuid().ToString()
                });

                var expected = new
                {
                    EmailAddress = emailAddress,
                    AccountId = createResponse.AccountId
                };

                var actual = await _systemUnderTest.GetAccountAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
