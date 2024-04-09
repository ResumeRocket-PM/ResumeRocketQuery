using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Repository.Tests
{
    public class ResumeRocketQueryRepositoryTests
    {
        private IResumeRocketQueryRepository _systemUnderTest;

        public ResumeRocketQueryRepositoryTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<FILL_THIS_IN>();
        }

        public class CreateAccountAsync : ResumeRocketQueryRepositoryTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_stored()
            {
                var accountId = await _systemUnderTest.CreateAccountAsync(new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    }
                });

                Assert.True(accountId > 0);
            }
        }
    }
}
