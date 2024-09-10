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
    public class EmailAddressDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public EmailAddressDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private IEmailAddressDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<IEmailAddressDataLayer>();
        }

        [Theory]
        [InlineData(typeof(EmailAddressDataLayer))]
        public async Task WHEN_InsertEmailAddressStorageAsync_is_called_THEN_email_address_is_stored(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });

            var emailAddressId = await systemUnderTest.InsertEmailAddressStorageAsync(new EmailAddressStorage
            {
                EmailAddress = "test@example.com",
                AccountId = accountId
            });

            Assert.True(emailAddressId > 0);
        }

        [Theory]
        [InlineData(typeof(EmailAddressDataLayer))]
        public async Task WHEN_InsertEmailAddressStorageAsync_is_called_THEN_storage_matches(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });

            var expected = new EmailAddressStorage
            {
                EmailAddress = "test@example.com",
                AccountId = accountId
            };

            var emailAddressId = await systemUnderTest.InsertEmailAddressStorageAsync(expected);
            expected.EmailAddressId = emailAddressId;

            var actual = await systemUnderTest.GetEmailAddressAsync(expected.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(EmailAddressDataLayer))]
        public async Task WHEN_UpdateEmailAddressStorageAsync_is_called_THEN_email_address_is_updated(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });

            var emailAddressId = await systemUnderTest.InsertEmailAddressStorageAsync(new EmailAddressStorage
            {
                EmailAddress = "test@example.com",
                AccountId = accountId
            });

            var updatedEmailAddress = new EmailAddressStorage
            {
                EmailAddressId = emailAddressId,
                EmailAddress = "updated@example.com",
                AccountId = accountId
            };

            await systemUnderTest.UpdateEmailAddressStorageAsync(updatedEmailAddress);

            var actual = await systemUnderTest.GetEmailAddressAsync(updatedEmailAddress.AccountId);

            updatedEmailAddress.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(EmailAddressDataLayer))]
        public async Task WHEN_GetEmailAddressAsync_is_called_THEN_correct_email_address_is_returned(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });

            var expected = new EmailAddressStorage
            {
                EmailAddress = "test@example.com",
                AccountId = accountId
            };

            var emailAddressId = await systemUnderTest.InsertEmailAddressStorageAsync(expected);

            expected.EmailAddressId = emailAddressId;

            var actual = await systemUnderTest.GetEmailAddressAsync(expected.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(EmailAddressDataLayer))]
        public async Task WHEN_GetAccountByEmailAddressAsync_is_called_THEN_correct_email_address_is_returned(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });

            var expected = new EmailAddressStorage
            {
                EmailAddress = "test@example.com",
                AccountId = accountId
            };

            var emailAddressId = await systemUnderTest.InsertEmailAddressStorageAsync(expected);

            expected.EmailAddressId = emailAddressId;

            var actual = await systemUnderTest.GetAccountByEmailAddressAsync(expected.EmailAddress);

            expected.ToExpectedObject().ShouldMatch(actual);
        }
    }
}
