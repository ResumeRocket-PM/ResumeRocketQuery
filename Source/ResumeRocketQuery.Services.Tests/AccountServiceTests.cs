using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Security.Principal;

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
                    EmailAddress = $"{Guid.NewGuid().ToString()}@gmail.com",
                    Password = Guid.NewGuid().ToString()
                });

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_matches_response()
            {
                var emailAddress = $"{Guid.NewGuid().ToString()}@gmail.com";

                var createResponse = await _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = Guid.NewGuid().ToString(),
                });

                var expected = new
                {
                    EmailAddress = emailAddress,
                    AccountId = createResponse.AccountId,

                    Title = account.Title,
                    Skills = account.Skills,
                    ProfilePhotoUrl = account.ProfilePhotoUrl,
                    PortfolioLink = account.PortfolioLink,
                    Experience = account.Experience,
                    Education = account.Education,
                    Location = account.Location,
                    Name = account.Name
                };

                var actual = await _systemUnderTest.GetAccountAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task GIVEN_account_exists_WHEN_CreateAccountAsync_is_called_THEN_validation_error_returned()
            {
                var emailAddress = $"{Guid.NewGuid().ToString()}@gmail.com";

                var createResponse = await _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = Guid.NewGuid().ToString()
                });

                var expected = new
                {
                    Message = "Account already exists"
                };

                var actual = await Assert.ThrowsAsync<ValidationException>(() => _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = Guid.NewGuid().ToString()
                }));

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
