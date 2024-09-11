using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;

namespace ResumeRocketQuery.DataLayerIntegrationTests
{
    [Rollback]
    public class SkillDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public SkillDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private ISkillDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<ISkillDataLayer>();
        }

        [Theory]
        [InlineData(typeof(SkillDataLayer))]
        public async Task WHEN_InsertSkillAsync_is_called_THEN_skill_is_stored(Type storageType)
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

            var skillId = await systemUnderTest.InsertSkillAsync(new SkillStorage
            {
                AccountId = accountId,
                Description = "C# Programming"
            });

            Assert.True(skillId > 0);
        }

        [Theory]
        [InlineData(typeof(SkillDataLayer))]
        public async Task WHEN_InsertSkillAsync_is_called_THEN_storage_matches(Type storageType)
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

            var insert = new SkillStorage
            {
                AccountId = accountId,
                Description = "C# Programming"
            };

            var skillId = await systemUnderTest.InsertSkillAsync(insert);

            insert.SkillId = skillId;

            var expected = new[]
            {
                insert
            };

            var actual = await systemUnderTest.GetSkillAsync(accountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(SkillDataLayer))]
        public async Task WHEN_DeleteSkillAsync_is_called_THEN_skill_is_deleted(Type storageType)
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

            var skillId = await systemUnderTest.InsertSkillAsync(new SkillStorage
            {
                AccountId = accountId,
                Description = "C# Programming"
            });

            await systemUnderTest.DeleteSkillAsync(skillId);

            var actual = await systemUnderTest.GetSkillAsync(skillId);

            Assert.Empty(actual);
        }
    }
}
