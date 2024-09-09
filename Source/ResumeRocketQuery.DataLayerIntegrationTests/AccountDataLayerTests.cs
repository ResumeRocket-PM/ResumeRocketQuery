using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;

namespace ResumeRocketQuery.DataLayerIntegrationTests
{
    [Rollback]
    public class AccountDataLayerTests
    {
        private IResumeRocketQueryStorage GetSystemUnderTest(Type storageType)
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            var config = serviceProvider.GetService<IResumeRocketQueryConfigurationSettings>();

            IResumeRocketQueryStorage systemUnderTest = (IResumeRocketQueryStorage)Activator.CreateInstance(storageType);

            return systemUnderTest;
        }

        public class InsertAccountStorageAsync : AccountDataLayerTests
        {
            [Theory]
            [InlineData(typeof(AccountDataLayer))]
            public async Task WHEN_InsertAccountStorageAsync_is_called_THEN_account_is_stored(Type storageType)
            {
                var systemUnderTest = (IAccountDataLayer)Activator.CreateInstance(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    ProfilePhotoLink = Guid.NewGuid().ToString(),
                    Title = Guid.NewGuid().ToString(),
                    StateLocation = Guid.NewGuid().ToString(),
                    PortfolioLink = Guid.NewGuid().ToString(),
                });

                Assert.True(accountId > 0);
            }

            [Theory]
            [InlineData(typeof(AccountDataLayer))]
            public async Task WHEN_InsertAccountStorageAsync_is_called_THEN_storage_matches(Type storageType)
            {
                var systemUnderTest = (IAccountDataLayer)Activator.CreateInstance(storageType);

                var expected = new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    ProfilePhotoLink = Guid.NewGuid().ToString(),
                    Title = Guid.NewGuid().ToString(),
                    StateLocation = Guid.NewGuid().ToString(),
                    PortfolioLink = Guid.NewGuid().ToString(),
                };

                var accountId = await systemUnderTest.InsertAccountStorageAsync(expected);
                expected.AccountId = accountId;

                var actual = await systemUnderTest.GetAccountAsync(accountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class GetAccountAsync : AccountDataLayerTests
        {
            [Theory]
            [InlineData(typeof(AccountDataLayer))]
            public async Task WHEN_GetAccountAsync_is_called_THEN_correct_account_is_returned(Type storageType)
            {
                var systemUnderTest = (IAccountDataLayer)Activator.CreateInstance(storageType);

                var expected = new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    ProfilePhotoLink = Guid.NewGuid().ToString(),
                    Title = Guid.NewGuid().ToString(),
                    StateLocation = Guid.NewGuid().ToString(),
                    PortfolioLink = Guid.NewGuid().ToString(),
                };

                var accountId = await systemUnderTest.InsertAccountStorageAsync(expected);
                expected.AccountId = accountId;

                var actual = await systemUnderTest.GetAccountAsync(accountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }

    public class UpdateAccountStorageAsync : AccountDataLayerTests
    {
        [Theory]
        [InlineData(typeof(AccountDataLayer))]
        public async Task WHEN_UpdateAccountStorageAsync_is_called_THEN_account_is_updated(Type storageType)
        {
            var systemUnderTest = (IAccountDataLayer)Activator.CreateInstance(storageType);

            var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });

            var updatedAccount = new AccountStorage
            {
                AccountId = accountId,
                AccountAlias = "UpdatedAlias",
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                ProfilePhotoLink = "UpdatedPhotoLink",
                Title = "UpdatedTitle",
                StateLocation = "UpdatedStateLocation",
                PortfolioLink = "UpdatedPortfolioLink",
            };

            await systemUnderTest.UpdateAccountStorageAsync(updatedAccount);

            var actual = await systemUnderTest.GetAccountAsync(accountId);

            updatedAccount.ToExpectedObject().ShouldMatch(actual);
        }
    }
}
