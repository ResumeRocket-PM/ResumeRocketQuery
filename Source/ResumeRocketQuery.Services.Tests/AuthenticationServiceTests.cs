using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class AuthenticationServiceTests
    {
        private IServiceProvider _serviceProvider;
        private IAuthenticationService _systemUnderTest;

        public AuthenticationServiceTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = _serviceProvider.GetService<IAuthenticationService>();
        }

        public class ValidateJsonWebToken : AuthenticationServiceTests
        {
            [Fact]
            public async Task GIVEN_a_valid_jwt_WHEN_ValidateJsonWebToken_is_called_THEN_jwt_created()
            {
                var accountService = _serviceProvider.GetService<IAccountService>();

                var emailAddress = $"{Guid.NewGuid().ToString()}@mailinator.com";
                var password = "password1";

                var accountId = await accountService.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = password
                });

                var jsonWebToken = _systemUnderTest.CreateJsonWebToken(accountId.AccountId);

                var actual = _systemUnderTest.ValidateJsonWebToken(jsonWebToken);

                Assert.True(actual);
            }

            [Fact]
            public async Task GIVEN_an_invalid_jwt_WHEN_ValidateJsonWebToken_is_called_THEN_jwt_created()
            {
                await Task.Delay(0);

                var actual = _systemUnderTest.ValidateJsonWebToken(Guid.NewGuid().ToString());

                Assert.False(actual);
            }
        }

        public class CreateJsonWebToken : AuthenticationServiceTests
        {
            [Fact]
            public async Task GIVEN_a_valid_account_WHEN_CreateJsonWebToken_is_called_THEN_jwt_created()
            {
                var accountService = _serviceProvider.GetService<IAccountService>();

                var emailAddress = $"{Guid.NewGuid().ToString()}@mailinator.com";
                var password = "password1";

                var accountId = await accountService.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = password
                });

                var actual = _systemUnderTest.CreateJsonWebToken(accountId.AccountId);

                Assert.NotNull(actual);
            }
        }

        public class AuthenticateAccountAsync : AuthenticationServiceTests
        {
            [Fact]
            public async Task GIVEN_account_does_not_exist_WHEN_AuthenticateAccountAsync_is_called_THEN_null_is_returned()
            {

                var expected = new
                {
                    IsAuthenticated = false,
                    JsonWebToken = Expect.Null()
                };

                var actual = await _systemUnderTest.AuthenticateAccountAsync(new AuthenticateAccountRequest
                {
                    EmailAddress = "tdebruin",
                    Password = "11111"
                });

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task GIVEN_account_does_exist_WHEN_AuthenticateAccountAsync_is_called_THEN_jwt_is_returned()
            {
                var accountService = _serviceProvider.GetService<IAccountService>();

                var emailAddress = $"{Guid.NewGuid().ToString()}@mailinator.com";
                var password = "password1";

                await accountService.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = password
                });

                var expected = new
                {
                    IsAuthenticated = true,
                    JsonWebToken = Expect.NotNull()
                };

                var actual = await _systemUnderTest.AuthenticateAccountAsync(new AuthenticateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = password
                });

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
