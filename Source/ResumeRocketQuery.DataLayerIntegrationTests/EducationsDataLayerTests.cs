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
    public class EducationsDataLayerTests
    {
        private IEducationDataLayer GetSystemUnderTest(Type storageType)
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            var config = serviceProvider.GetService<IResumeRocketQueryConfigurationSettings>();

            IEducationDataLayer systemUnderTest = (IEducationDataLayer)Activator.CreateInstance(storageType);

            return systemUnderTest;
        }

        [Theory]
        [InlineData(typeof(EducationsDataLayer))]
        public async Task WHEN_InsertEducationStorageAsync_is_called_THEN_education_is_stored(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var educationId = await systemUnderTest.InsertEducationStorageAsync(new EducationStorage
            {
                AccountId = 1,
                SchoolName = "Test School",
                Degree = "Bachelor's",
                Major = "Computer Science",
                Minor = "Mathematics",
                GraduationDate = DateTime.Now
            });

            Assert.True(educationId > 0);
        }

        [Theory]
        [InlineData(typeof(EducationsDataLayer))]
        public async Task WHEN_InsertEducationStorageAsync_is_called_THEN_storage_matches(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var insertRequest = new EducationStorage
            {
                AccountId = 1,
                SchoolName = "Test School",
                Degree = "Bachelor's",
                Major = "Computer Science",
                Minor = "Mathematics",
                GraduationDate = DateTime.Now
            };

            var educationId = await systemUnderTest.InsertEducationStorageAsync(insertRequest);
            insertRequest.EducationId = educationId;

            var expected = new[]
            {
                insertRequest
            };

            var actual = await systemUnderTest.GetEducationAsync(educationId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(EducationsDataLayer))]
        public async Task WHEN_UpdateEducationStorageAsync_is_called_THEN_education_is_updated(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var educationId = await systemUnderTest.InsertEducationStorageAsync(new EducationStorage
            {
                AccountId = 1,
                SchoolName = "Test School",
                Degree = "Bachelor's",
                Major = "Computer Science",
                Minor = "Mathematics",
                GraduationDate = DateTime.Now
            });

            var updatedEducation = new EducationStorage
            {
                EducationId = educationId,
                AccountId = 1,
                SchoolName = "Updated School",
                Degree = "Master's",
                Major = "Software Engineering",
                Minor = "None",
                GraduationDate = DateTime.Now.AddYears(1)
            };

            await systemUnderTest.UpdateEducationStorageAsync(updatedEducation);

            var expected = new[]
{
                updatedEducation
            };

            var actual = await systemUnderTest.GetEducationAsync(educationId);

            updatedEducation.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(EducationsDataLayer))]
        public async Task WHEN_DeleteEducationStorageAsync_is_called_THEN_correct_education_is_deleted(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var expected = new EducationStorage
            {
                AccountId = 1,
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
    }
}
