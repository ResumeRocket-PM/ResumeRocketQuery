using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Helper;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class AuthenticationHelperTests
    {
        private IServiceProvider _serviceProvider;
        private IAuthenticationHelper _systemUnderTest;

        public AuthenticationHelperTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = _serviceProvider.GetService<IAuthenticationHelper>();
        }

        public class GeneratePasswordHashAsync : AuthenticationHelperTests
        {
            [Fact]
            public async Task WHEN_GeneratePasswordHashAsync_is_called_THEN_password_hashed()
            {
                var passwordHashRequest = new PasswordHashRequest
                {
                    Password = "password1!",
                    Salt = "salt"
                };

                var expected = new
                {
                    HashedPassword = "BUgrvPLOqvu6/WqVgBkUiuJuP39ex9kZUIji68AYQOPavEvy+g/UiGb1VtzQmVzLPgzXQPgqhglPTdocdMxbUw=="
                };

                var actual = await _systemUnderTest.GeneratePasswordHashAsync(passwordHashRequest);

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task GIVEN_a_password_is_hashed_WHEN_GeneratePasswordHashAsync_is_called_THEN_passwords_match()
            {

                var passwordHashRequest = new PasswordHashRequest
                {
                    Password = "password1!",
                    Salt = Guid.NewGuid().ToString()
                };

                var expected = await _systemUnderTest.GeneratePasswordHashAsync(passwordHashRequest);

                var actual = await _systemUnderTest.GeneratePasswordHashAsync(passwordHashRequest);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
