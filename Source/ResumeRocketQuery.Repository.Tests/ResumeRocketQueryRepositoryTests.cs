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

            _systemUnderTest = serviceProvider.GetService<IResumeRocketQueryRepository>();
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

            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_storage_matches()
            {
                var expected = new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    }
                };

                expected.AccountId = await _systemUnderTest.CreateAccountAsync(expected);

                var actual = await _systemUnderTest.GetAccountAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class GetAccountByEmailAddressAsync : ResumeRocketQueryRepositoryTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_storage_matches()
            {
                var expected = new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    }
                };

                expected.AccountId = await _systemUnderTest.CreateAccountAsync(expected);

                var actual = await _systemUnderTest.GetAccountByEmailAddressAsync(expected.EmailAddress);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }


        public class CreatePortfolioAsync : ResumeRocketQueryRepositoryTests
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

                var portfolioId = await _systemUnderTest.CreatePortfolioAsync(new Portfolio
                {
                    Configuration = Guid.NewGuid().ToString(),
                    AccountId = accountId,
                });

                Assert.True(portfolioId > 0);
            }

            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_storage_matches()
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

                var portfolio = Guid.NewGuid().ToString();

                var portfolioId = await _systemUnderTest.CreatePortfolioAsync(new Portfolio
                {
                    Configuration = portfolio,
                    AccountId = accountId,
                });

                var expected = new Portfolio
                {
                    AccountId = accountId,
                    Configuration = portfolio
                };

                var actual = await _systemUnderTest.GetPortfolioAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
