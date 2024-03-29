using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Fakes;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Storage.Tests
{
    public class ResumeRocketQueryStorageTests
    {
        private IResumeRocketQueryStorage GetSystemUnderTest(Type storageType)
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            var config = serviceProvider.GetService<IResumeRocketQueryConfigurationSettings>();

            IResumeRocketQueryStorage systemUnderTest;

            if (storageType == typeof(DapperResumeRocketQueryStorage))
            {
                systemUnderTest = (IResumeRocketQueryStorage) Activator.CreateInstance(storageType, config);
            }
            else
            {
                systemUnderTest = (IResumeRocketQueryStorage) Activator.CreateInstance(storageType);
            }

            return systemUnderTest;
        }

        public class InsertEmailAddressStorageAsync : ResumeRocketQueryStorageTests
        {
            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_InsertEmailAddressStorageAsync_is_called_THEN_account_is_stored(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                var emailId = await systemUnderTest.InsertEmailAddressStorageAsync(new EmailAddressStorage
                {
                    EmailAddress = Guid.NewGuid().ToString(),
                    AccountId = accountId
                });

                Assert.True(emailId > 0);
            }

            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_InsertEmailAddressStorageAsync_is_called_THEN_storage_matches(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                var expected = new EmailAddressStorage
                {
                    EmailAddress = Guid.NewGuid().ToString(),
                    AccountId = accountId
                };

                expected.EmailAddressId = await systemUnderTest.InsertEmailAddressStorageAsync(expected);

                var actual = await systemUnderTest.SelectEmailAddressStorageAsync(expected.EmailAddressId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class InsertAccountStorageAsync : ResumeRocketQueryStorageTests
        {
            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_InsertAccountStorageAsync_is_called_THEN_account_is_stored(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                Assert.True(accountId > 0);
            }

            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_InsertAccountStorageAsync_is_called_THEN_storage_matches(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var expected = new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                };

                var accountId = await systemUnderTest.InsertAccountStorageAsync(expected);

                expected.AccountId = accountId;

                var actual = await systemUnderTest.SelectAccountStorageAsync(accountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class SelectEmailAddressStorageByEmailAddressAsync : ResumeRocketQueryStorageTests
        {
            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_SelectEmailAddressStorageByEmailAddressAsync_is_called_THEN_storage_matches(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                var expected = new EmailAddressStorage
                {
                    EmailAddress = Guid.NewGuid().ToString(),
                    AccountId = accountId
                };

                var emailAddressId = await systemUnderTest.InsertEmailAddressStorageAsync(expected);

                expected.EmailAddressId = emailAddressId;

                var actual = await systemUnderTest.SelectEmailAddressStorageByEmailAddressAsync(expected.EmailAddress);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class SelectEmailAddressStorageByAccountIdAsync : ResumeRocketQueryStorageTests
        {
            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_SelectEmailAddressStorageByEmailAddressAsync_is_called_THEN_storage_matches(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                var expected = new EmailAddressStorage
                {
                    EmailAddress = Guid.NewGuid().ToString(),
                    AccountId = accountId
                };

                var emailAddressId = await systemUnderTest.InsertEmailAddressStorageAsync(expected);

                expected.EmailAddressId = emailAddressId;

                var actual = await systemUnderTest.SelectEmailAddressStorageByAccountIdAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
