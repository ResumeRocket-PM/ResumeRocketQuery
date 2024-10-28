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
        public async Task GIVEN_original_resumeId_set_WHEN_InsertResumeAsync_is_called_THEN_version_is_incremented(Type storageType)
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today,
            });

            var expected = new ResumeStorage
            {
                AccountId = accountId,
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today,
                OriginalResumeID = resumeId,
                OriginalResume = false
            };

            var resumeId2 = await systemUnderTest.InsertResumeAsync(expected);

            expected.Version = 1;
            expected.ResumeId = resumeId2;

            var actual = await systemUnderTest.GetResumeAsync(resumeId2);

            expected.ToExpectedObject().ShouldMatch(actual);
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
            });

            var updatedResume = new ResumeStorage
            {
                ResumeId = resumeId,
                AccountId = accountId,
                Resume = "Updated Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today,
                Version = 1
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
            };

            var resumeId = await systemUnderTest.InsertResumeAsync(expected);

            expected.ResumeId = resumeId;

            var actual = await systemUnderTest.GetResumeAsync(expected.ResumeId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_InsertResumeChangeAsync_is_called_THEN_resume_change_is_stored(Type storageType)
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
            });

            var resumeChange = new ResumeChangesStorage
            {
                ResumeId = resumeId,
                OriginalText = "Original text",
                ModifiedText = "Modified text",
                ExplanationString = "Explanation",
                Accepted = false,
                HtmlID = "<div>this is test html</div>"
            };

            await systemUnderTest.InsertResumeChangeAsync(resumeChange);

            var storedChanges = await systemUnderTest.SelectResumeChangesAsync(resumeChange.ResumeId);

            Assert.Contains(storedChanges, change =>
                change.OriginalText == resumeChange.OriginalText &&
                change.ModifiedText == resumeChange.ModifiedText &&
                change.ExplanationString == resumeChange.ExplanationString &&
                change.Accepted == resumeChange.Accepted &&
                change.HtmlID == resumeChange.HtmlID
            );
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_InsertResumeChangeAsync_is_called_THEN_id_is_correct(Type storageType)
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
            });

            var resumeChange = new ResumeChangesStorage
            {
                ResumeId = resumeId,
                OriginalText = "Original text",
                ModifiedText = "Modified text",
                ExplanationString = "Explanation",
                Accepted = false,
                HtmlID = "<div>this is test html</div>"
            };

            var actual = await systemUnderTest.InsertResumeChangeAsync(resumeChange);

            Assert.True(actual > 0);
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_SelectResumeChangesAsync_is_called_THEN_resume_changes_are_retrieved(Type storageType)
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
            });

            var resumeChange = new ResumeChangesStorage
            {
                ResumeId = resumeId,
                OriginalText = "Original text for selection",
                ModifiedText = "Modified text for selection",
                ExplanationString = "Explanation for selection",
                Accepted = false,
                HtmlID = "htmlSelect123"
            };
            await systemUnderTest.InsertResumeChangeAsync(resumeChange);

            var changes = await systemUnderTest.SelectResumeChangesAsync(resumeId);

            Assert.NotEmpty(changes);
            Assert.Contains(changes, change => change.HtmlID == "htmlSelect123");
        }

        [Theory]
        [InlineData(typeof(ResumeDataLayer))]
        public async Task WHEN_UpdateResumeChangeAsync_is_called_THEN_resume_change_is_updated(Type storageType)
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
                Resume = "Sample Resume Text",
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
            });

            var resumeChange = new ResumeChangesStorage
            {
                ResumeId = resumeId,
                OriginalText = "Text before update",
                ModifiedText = "Text after update",
                ExplanationString = "Update explanation",
                Accepted = false,
                HtmlID = "htmlUpdate123"
            };

            await systemUnderTest.InsertResumeChangeAsync(resumeChange);

            var storedChanges = await systemUnderTest.SelectResumeChangesAsync(resumeChange.ResumeId);

            var changeToUpdate = storedChanges.FirstOrDefault(change => change.HtmlID == resumeChange.HtmlID);

            changeToUpdate.Accepted = true;

            await systemUnderTest.UpdateResumeChangeAsync(changeToUpdate);

            var updatedChanges = await systemUnderTest.SelectResumeChangesAsync(changeToUpdate.ResumeId);

            Assert.Contains(updatedChanges, change =>
                change.HtmlID == changeToUpdate.HtmlID &&
                change.Accepted == true
            );
        }
    }


}
