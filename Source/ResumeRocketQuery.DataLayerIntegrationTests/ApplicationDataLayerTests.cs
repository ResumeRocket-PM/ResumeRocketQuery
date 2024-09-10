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
    public class ApplicationDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public ApplicationDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private IApplicationDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<IApplicationDataLayer>();
        }


        [Theory]
        [InlineData(typeof(ApplicationDataLayer))]
        public async Task WHEN_InsertApplicationAsync_is_called_THEN_application_is_stored(Type storageType)
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

            var applicationId = await systemUnderTest.InsertApplicationAsync(new ApplicationStorage
            {
                AccountId = accountId,
                ApplyDate = DateTime.UtcNow,
                Status = "Pending",
                Position = "Software Engineer",
                CompanyName = "Tech Corp",
                JobPostingUrl = Guid.NewGuid().ToString(),
            });

            Assert.True(applicationId > 0);
        }

        [Theory]
        [InlineData(typeof(ApplicationDataLayer))]
        public async Task WHEN_GetApplicationAsync_is_called_THEN_storage_matches(Type storageType)
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

            var insertRequest = new ApplicationStorage
            {
                AccountId = accountId,
                ApplyDate = DateTime.Today,
                Status = "Pending",
                Position = "Software Engineer",
                CompanyName = "Tech Corp",
                JobPostingUrl = Guid.NewGuid().ToString(),
            };

            var applicationId = await systemUnderTest.InsertApplicationAsync(insertRequest);

            insertRequest.ApplicationId = applicationId;

            var expected = new
            {
                AccountId = insertRequest.AccountId,
                ApplyDate = insertRequest.ApplyDate,
                Status = insertRequest.Status,
                Position = insertRequest.Position,
                CompanyName = insertRequest.CompanyName,
                JobPostingUrl = insertRequest.JobPostingUrl,
                ApplicationId = insertRequest.ApplicationId
            };

            var actual = await systemUnderTest.GetApplicationAsync(insertRequest.ApplicationId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(ApplicationDataLayer))]
        public async Task WHEN_InsertApplicationAsync_is_called_THEN_storage_matches(Type storageType)
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

            var insertRequest = new ApplicationStorage
            {
                AccountId = accountId,
                ApplyDate = DateTime.Today,
                Status = "Pending",
                Position = "Software Engineer",
                CompanyName = "Tech Corp",
                JobPostingUrl = Guid.NewGuid().ToString(),
            };

            var applicationId = await systemUnderTest.InsertApplicationAsync(insertRequest);
            insertRequest.ApplicationId = applicationId;

            var expected = new[]
            {
                new
                {
                    AccountId = insertRequest.AccountId,
                    ApplyDate = insertRequest.ApplyDate,
                    Status = insertRequest.Status,
                    Position = insertRequest.Position,
                    CompanyName = insertRequest.CompanyName,
                    JobPostingUrl = insertRequest.JobPostingUrl
                }
            };

            var actual = await systemUnderTest.GetApplicationsAsync(accountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(ApplicationDataLayer))]
        public async Task WHEN_UpdateApplicationAsync_is_called_THEN_application_is_updated(Type storageType)
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

            var applicationId = await systemUnderTest.InsertApplicationAsync(new ApplicationStorage
            {
                AccountId = accountId,
                ApplyDate = DateTime.Today,
                Status = "Pending",
                Position = "Software Engineer",
                CompanyName = "Tech Corp"
            });

            var updatedApplication = new ApplicationStorage
            {
                ApplyDate = DateTime.Today,
                Position = "Software Engineer",
                CompanyName = "Tech Corp",
                ApplicationId = applicationId,
                Status = "Accepted",
                AccountId = accountId
            };

            await systemUnderTest.UpdateApplicationAsync(updatedApplication);

            var expected = new[]
            {
                new
                {
                    AccountId = updatedApplication.AccountId,
                    ApplyDate = updatedApplication.ApplyDate,
                    Status = updatedApplication.Status,
                    Position = updatedApplication.Position,
                    CompanyName = updatedApplication.CompanyName,
                    ApplicationId = updatedApplication.ApplicationId,
                    JobPostingUrl = updatedApplication.JobPostingUrl
                }
            };

            var actual = await systemUnderTest.GetApplicationsAsync(accountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }
    }
}
