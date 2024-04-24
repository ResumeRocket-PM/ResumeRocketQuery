using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Fakes;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Sprache;
using ResumeRocketQuery.Domain.Services;
using System.Globalization;

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

        public class InsertPortfolioStorageAsync : ResumeRocketQueryStorageTests
        {
            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            //[InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_InsertEmailAddressStorageAsync_is_called_THEN_account_is_stored(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                var portfolioId = await systemUnderTest.InsertPortfolioStorageAsync(new PortfolioStorage
                {
                    PortfolioAlias = Guid.NewGuid().ToString(),
                    PortfolioConfiguration = Guid.NewGuid().ToString(),
                    AccountId = accountId
                });

                Assert.True(portfolioId > 0);
            }

            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            //[InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_InsertEmailAddressStorageAsync_is_called_THEN_storage_matches(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                var expected = new PortfolioStorage
                {
                    PortfolioAlias = Guid.NewGuid().ToString(),
                    PortfolioConfiguration = Guid.NewGuid().ToString(),
                    AccountId = accountId
                };

                expected.PortfolioId = await systemUnderTest.InsertPortfolioStorageAsync(expected);

                var actual = await systemUnderTest.SelectPortfolioStorageAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
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

        // for resume

        public class InsertResume : ResumeRocketQueryStorageTests
        {
            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task when_insert_then_ID_return(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                var resumeID = await systemUnderTest.InsertResumeStorageAsync(new ResumeStorage
                {
                    applyDate = DateTime.Now,
                    jobUrl = Guid.NewGuid().ToString(),
                    accountID = accountId,
                    status = Guid.NewGuid().ToString(),
                    resume = "test",
                    position = "A",
                    companyName = Guid.NewGuid().ToString(),

                });

                Assert.True(resumeID > 0);
            }

            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task when_Select_resume_by_ID_return_the_resume(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                DateTime firstDate = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                ;
                string firstJobUrl = Guid.NewGuid().ToString();
                int firstAcID = accountId;
                string firstStatus = Guid.NewGuid().ToString();
                string firstResumeText = "adobe resume";
                string firstCompanyName = "Adobe";
                var expected1 = new ResumeStorage
                {

                    applyDate = firstDate,
                    jobUrl = firstJobUrl,
                    accountID = accountId,
                    status = firstStatus,
                    resume= firstResumeText,
                    position = "A",
                    companyName = firstCompanyName,

                };

                DateTime secondDate = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string secondJobUrl = Guid.NewGuid().ToString();
                int secondAcID = accountId;
                string secondStatus = Guid.NewGuid().ToString();
                string secondResumeText = "adobe resume";
                string secondCompanyName = "Adobe";
                var expected2 = new ResumeStorage
                {
                    applyDate = secondDate,
                    jobUrl = secondJobUrl,
                    accountID = accountId,
                    status = secondStatus,
                    resume= secondResumeText,
                    position = "B",
                    companyName = secondCompanyName,
                };

                await systemUnderTest.InsertResumeStorageAsync(expected1);
                await systemUnderTest.InsertResumeStorageAsync(expected2);

                var actual = await systemUnderTest.SelectResumeStoragesAsync(accountId);

                Assert.True(actual.Count == 2);

                Assert.Equal(firstDate, actual[0].applyDate);
                Assert.Equal(firstJobUrl, actual[0].jobUrl);
                Assert.Equal(firstAcID, actual[0].accountID);
                Assert.Equal(firstStatus, actual[0].status);
                Assert.Equal(firstResumeText, actual[0].resume);
                Assert.Equal(firstCompanyName, actual[0].companyName);

                Assert.Equal(secondDate, actual[1].applyDate);
                Assert.Equal(secondJobUrl, actual[1].jobUrl);
                Assert.Equal(secondAcID, actual[1].accountID);
                Assert.Equal(secondStatus, actual[1].status);
                Assert.Equal(secondResumeText, actual[1].resume);
                Assert.Equal(secondCompanyName, actual[1].companyName);

                //expected2.ToExpectedObject().ShouldMatch(actual[0]);
                //expected1.ToExpectedObject().ShouldMatch(actual[1]);
            }

            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task Resume_status_change_correct(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                string beforeUpdate = "before Update status";
                var resumeID = await systemUnderTest.InsertResumeStorageAsync(new ResumeStorage
                {
                    applyDate = DateTime.Now,
                    jobUrl = Guid.NewGuid().ToString(),
                    accountID = accountId,
                    status = beforeUpdate,
                    resume = "test",
                    position = "A",
                    companyName = Guid.NewGuid().ToString(),

                });
                string afterUpdate = "after Update status";

                ResumeStorage update = new();
                update.ResumeID = resumeID;
                update.status = afterUpdate;
                await systemUnderTest.UpdateResumeStorageAsync(update);

                var rs = await systemUnderTest.SelectResumeStorageAsync(resumeID);
                Assert.Equal(afterUpdate,rs.status);
                Assert.NotEqual(beforeUpdate,rs.status);

            }
        }

        public class SelectResumeStorageAsync : ResumeRocketQueryStorageTests
        {
            [Theory]
            [InlineData(typeof(DapperResumeRocketQueryStorage))]
            [InlineData(typeof(MemoryResumeRocketQueryStorage))]
            public async Task WHEN_SelectResumeStorageAsync_THEN_storage_is_returned(Type storageType)
            {
                var systemUnderTest = GetSystemUnderTest(storageType);

                var accountId = await systemUnderTest.InsertAccountStorageAsync(new AccountStorage
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    AccountConfiguration = Guid.NewGuid().ToString()
                });

                var expected = new ResumeStorage
                {
                    applyDate = DateTime.Today,
                    jobUrl = Guid.NewGuid().ToString(),
                    accountID = accountId,
                    status = Guid.NewGuid().ToString(),
                    resume = "test",
                    position = "A",
                    companyName = Guid.NewGuid().ToString(),
                };

                expected.ResumeID = await systemUnderTest.InsertResumeStorageAsync(expected);

                var actual = await systemUnderTest.SelectResumeStorageAsync(expected.ResumeID);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }


    }
}
