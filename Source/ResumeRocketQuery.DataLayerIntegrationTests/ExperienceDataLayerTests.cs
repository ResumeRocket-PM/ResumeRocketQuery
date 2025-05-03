using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;

namespace ResumeRocketQuery.DataLayerIntegrationTests
{
    [Rollback]
    public class ExperienceDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public ExperienceDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private IExperienceDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<IExperienceDataLayer>();
        }

        [Theory]
        [InlineData(typeof(ExperienceDataLayer))]
        public async Task WHEN_InsertExperienceAsync_is_called_THEN_experience_is_stored(Type storageType)
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

            var experienceId = await systemUnderTest.InsertExperienceAsync(new ExperienceStorage
            {
                AccountId = accountId,
                Company = "Tech Corp",
                Position = "Software Engineer",
                Type = "Full-time",
                Description = "Worked on building enterprise software.",
                StartDate = DateTime.UtcNow.AddYears(-3),
                EndDate = DateTime.UtcNow.AddYears(-1)
            });

            Assert.True(experienceId > 0);
        }

        [Theory]
        [InlineData(typeof(ExperienceDataLayer))]
        public async Task WHEN_InsertExperienceAsync_is_called_THEN_storage_matches(Type storageType)
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

            var request = new ExperienceStorage
            {
                AccountId = accountId,
                Company = "Tech Corp",
                Position = "Software Engineer",
                Type = "Full-time",
                Description = "Worked on building enterprise software.",
                StartDate = DateTime.Today.AddYears(-3),
                EndDate = DateTime.Today.AddYears(-1)
            };

            var experienceId = await systemUnderTest.InsertExperienceAsync(request);

            request.ExperienceId = experienceId;

            var expected = new[]
            {
                new
                {
                    AccountId = request.AccountId,
                    Company = request.Company,
                    Position = request.Position,
                    Type = request.Type,
                    Description = request.Description,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                }
            };

            var actual = await systemUnderTest.GetExperienceAsync(request.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(ExperienceDataLayer))]
        public async Task WHEN_UpdateExperienceAsync_is_called_THEN_experience_is_updated(Type storageType)
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

            var experienceId = await systemUnderTest.InsertExperienceAsync(new ExperienceStorage
            {
                AccountId = accountId,
                Company = "Tech Corp",
                Position = "Software Engineer",
                Type = "Full-time",
                Description = "Worked on building enterprise software.",
                StartDate = DateTime.Today.AddYears(-3),
                EndDate = DateTime.Today.AddYears(-1)
            });

            var updatedExperience = new ExperienceStorage
            {
                ExperienceId = experienceId,
                AccountId = accountId,
                Company = "Tech Corp",
                Position = "Senior Software Engineer",
                Type = "Full-time",
                Description = "Promoted and led a team of developers.",
                StartDate = DateTime.Today.AddYears(-4),
                EndDate = DateTime.Today
            };

            await systemUnderTest.UpdateExperienceAsync(updatedExperience);

            var expected = new[]
            {
            new
            {
                ExperienceId = updatedExperience.ExperienceId,
                AccountId = updatedExperience.AccountId,
                Company = updatedExperience.Company,
                Position = updatedExperience.Position,
                Type = updatedExperience.Type,
                Description = updatedExperience.Description,
                StartDate = updatedExperience.StartDate,
                EndDate = updatedExperience.EndDate
            }
        };

            var actual = await systemUnderTest.GetExperienceAsync(updatedExperience.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(ExperienceDataLayer))]
        public async Task WHEN_DeleteExperienceAsync_is_called_THEN_experience_is_deleted(Type storageType)
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

            var experienceId = await systemUnderTest.InsertExperienceAsync(new ExperienceStorage
            {
                AccountId = accountId,
                Company = "Tech Corp",
                Position = "Software Engineer",
                Type = "Full-time",
                Description = "Worked on building enterprise software.",
                StartDate = DateTime.UtcNow.AddYears(-3),
                EndDate = DateTime.UtcNow.AddYears(-1)
            });

            await systemUnderTest.DeleteExperienceAsync(experienceId);

            var actual = await systemUnderTest.GetExperienceAsync(experienceId);

            Assert.Empty(actual);
        }

        [Theory]
        [InlineData(typeof(ExperienceDataLayer))]
        public async Task WHEN_DeleteExperienceByAccountIdAsync_is_called_THEN_experience_is_deleted(Type storageType)
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

            var experienceId = await systemUnderTest.InsertExperienceAsync(new ExperienceStorage
            {
                AccountId = accountId,
                Company = "Tech Corp",
                Position = "Software Engineer",
                Type = "Full-time",
                Description = "Worked on building enterprise software.",
                StartDate = DateTime.UtcNow.AddYears(-3),
                EndDate = DateTime.UtcNow.AddYears(-1)
            });

            await systemUnderTest.DeleteExperienceByAccountIdAsync(accountId);

            var actual = await systemUnderTest.GetExperienceAsync(experienceId);

            Assert.Empty(actual);
        }
    }
}
