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
    public class ResumeDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public ResumeDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private IResumeDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<IResumeDataLayer>();
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_InsertResumeAsync_is_called_THEN_resume_is_stored(Type storageType)
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

            var resumeId = await systemUnderTest.InsertResumeAsync(new ResumeStorage
            {
                AccountId = accountId,
                Resume = "Sample Resume Text"
            });

            Assert.True(resumeId > 0);
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_InsertResumeAsync_is_called_THEN_storage_matches(Type storageType)
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

            var insertRequest = new ResumeStorage
            {
                AccountId = accountId,
                Resume = "Sample Resume Text"
            };

            var resumeId = await systemUnderTest.InsertResumeAsync(insertRequest);
            insertRequest.ResumeId = resumeId;

            var expected = new[]
            {
                insertRequest
            };

            var actual = await systemUnderTest.GetResumesAsync(insertRequest.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_UpdateResumeAsync_is_called_THEN_resume_is_updated(Type storageType)
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

            var resumeId = await systemUnderTest.InsertResumeAsync(new ResumeStorage
            {
                AccountId = accountId,
                Resume = "Sample Resume Text"
            });

            var updatedResume = new ResumeStorage
            {
                ResumeId = resumeId,
                AccountId = accountId,
                Resume = "Updated Resume Text",
            };

            var expected = new[]
            {
                updatedResume
            };

            await systemUnderTest.UpdateResumeAsync(updatedResume);

            var actual = await systemUnderTest.GetResumesAsync(updatedResume.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_DeleteResumeAsync_is_called_THEN_resume_is_deleted(Type storageType)
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

            var resumeId = await systemUnderTest.InsertResumeAsync(new ResumeStorage
            {
                AccountId = accountId,
                Resume = "Sample Resume Text"
            });

            await systemUnderTest.DeleteResumeAsync(resumeId);

            var actual = await systemUnderTest.GetResumeAsync(resumeId);

            Assert.Null(actual);
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_GetResumeAsync_is_called_THEN_storage_matches(Type storageType)
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

            var expected = new ResumeStorage
            {
                AccountId = accountId,
                Resume = "Sample Resume Text"
            };

            var resumeId = await systemUnderTest.InsertResumeAsync(expected);

            expected.ResumeId = resumeId;

            var actual = await systemUnderTest.GetResumeAsync(expected.ResumeId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }
    }


}
