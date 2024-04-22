using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class PortfolioServiceTests
    {
        private readonly IPortfolioService _systemUnderTest;
        private readonly IAccountService _accountService;

        public PortfolioServiceTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IPortfolioService>();
            _accountService = serviceProvider.GetService<IAccountService>();
        }

        public class CreateAccountAsync : PortfolioServiceTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_created()
            {
                var account = await _accountService.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = $"{Guid.NewGuid().ToString()}@gmail.com",
                    Password = Guid.NewGuid().ToString()
                });

                var portfolio = Guid.NewGuid().ToString();

                var expected = new Portfolio
                {
                    AccountId = account.AccountId,
                    Configuration = portfolio
                };

                await _systemUnderTest.CreatePortfolio(expected);

                var actual = await _systemUnderTest.GetPortfolio(account.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }

        }
    }
}
