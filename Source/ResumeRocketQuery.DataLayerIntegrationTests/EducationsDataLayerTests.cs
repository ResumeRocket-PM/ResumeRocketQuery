using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Tests.Helpers;
using System;
using Xunit;

namespace ResumeRocketQuery.DataLayerIntegrationTests
{
    [Rollback]
    public class EducationsDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public EducationsDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private IEducationDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<IEducationDataLayer>();
        }

        [Theory]
        [InlineData(typeof(EducationDataLayer))]
        public async Task WHEN_InsertEducationStorageAsync_is_called_THEN_education_is_stored(Type storageType)
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

            var educationId = await systemUnderTest.InsertEducationStorageAsync(new EducationStorage
            {
                AccountId = accountId,
                SchoolName = "Test School",
                Degree = "Bachelor's",
                Major = "Computer Science",
                Minor = "Mathematics",
                GraduationDate = DateTime.Now
            });

            Assert.True(educationId > 0);
        }

        [Theory]
        [InlineData(typeof(EducationDataLayer))]
        public async Task WHEN_InsertEducationStorageAsync_is_called_THEN_storage_matches(Type storageType)
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

            var insertRequest = new EducationStorage
            {
                AccountId = accountId,
                SchoolName = "Test School",
                Degree = "Bachelor's",
                Major = "Computer Science",
                Minor = "Mathematics",
                GraduationDate = DateTime.Today
            };

            var educationId = await systemUnderTest.InsertEducationStorageAsync(insertRequest);
            insertRequest.EducationId = educationId;

            var expected = new[]
            {
                new 
                {
                    GraduationDate = insertRequest.GraduationDate,
                    Minor = insertRequest.Minor,
                    Major= insertRequest.Major,
                    Degree= insertRequest.Degree,
                    SchoolName= insertRequest.SchoolName,
                    AccountId= accountId,
                    EducationId = insertRequest.EducationId
                }
            };

            var actual = await systemUnderTest.GetEducationAsync(accountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(EducationDataLayer))]
        public async Task WHEN_UpdateEducationStorageAsync_is_called_THEN_education_is_updated(Type storageType)
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

            var educationId = await systemUnderTest.InsertEducationStorageAsync(new EducationStorage
            {
                AccountId = accountId,
                SchoolName = "Test School",
                Degree = "Bachelor's",
                Major = "Computer Science",
                Minor = "Mathematics",
                GraduationDate = DateTime.Today
            });

            var updatedEducation = new EducationStorage
            {
                EducationId = educationId,
                AccountId = accountId,
                SchoolName = "Updated School",
                Degree = "Master's",
                Major = "Software Engineering",
                Minor = "None",
                GraduationDate = DateTime.Today.AddYears(1)
            };

            await systemUnderTest.UpdateEducationStorageAsync(updatedEducation);

            var expected = new[]
{
                new
                {
                    EducationId = updatedEducation.EducationId,
                    AccountId = updatedEducation.AccountId,
                    SchoolName = updatedEducation.SchoolName,
                    Degree = updatedEducation.Degree,
                    Major = updatedEducation.Major,
                    Minor = updatedEducation.Minor,
                    GraduationDate = updatedEducation.GraduationDate
                }
            };

            var actual = await systemUnderTest.GetEducationAsync(accountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(EducationDataLayer))]
        public async Task WHEN_DeleteEducationStorageAsync_is_called_THEN_correct_education_is_deleted(Type storageType)
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

            var expected = new EducationStorage
            {
                AccountId = accountId,
                SchoolName = "Test School",
                Degree = "Bachelor's",
                Major = "Computer Science",
                Minor = "Mathematics",
                GraduationDate = DateTime.Now
            };

            var educationId = await systemUnderTest.InsertEducationStorageAsync(expected);

            await systemUnderTest.DeleteEducationStorageAsync(educationId);

            var education = await systemUnderTest.GetEducationAsync(expected.AccountId);

            Assert.Empty(education);
        }

        [Theory]
        [InlineData(typeof(EducationDataLayer))]
        public async Task WHEN_DeleteEducationStorageByAccountAsync_is_called_THEN_correct_education_is_deleted(Type storageType)
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

            var expected = new EducationStorage
            {
                AccountId = accountId,
                SchoolName = "Test School",
                Degree = "Bachelor's",
                Major = "Computer Science",
                Minor = "Mathematics",
                GraduationDate = DateTime.Now
            };

            var educationId = await systemUnderTest.InsertEducationStorageAsync(expected);

            await systemUnderTest.DeleteEducationStorageByAccountAsync(accountId);

            var education = await systemUnderTest.GetEducationAsync(expected.AccountId);

            Assert.Empty(education);
        }
    }
}
