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
    public class LoginDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public LoginDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private ILoginDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<ILoginDataLayer>();
        }

        [Theory]
        [InlineData(typeof(LoginDataLayer))]
        public async Task WHEN_InsertLoginStorageAsync_is_called_THEN_login_is_stored(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var loginId = await systemUnderTest.InsertLoginStorageAsync(new LoginStorage
            {
                AccountId = 1,
                Salt = "randomSalt",
                Hash = "hashedPassword"
            });

            Assert.True(loginId > 0);
        }

        [Theory]
        [InlineData(typeof(LoginDataLayer))]
        public async Task WHEN_InsertLoginStorageAsync_is_called_THEN_storage_matches(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var expected = new LoginStorage
            {
                AccountId = 1,
                Salt = "randomSalt",
                Hash = "hashedPassword"
            };

            var loginId = await systemUnderTest.InsertLoginStorageAsync(expected);
            expected.LoginId = loginId;

            var actual = await systemUnderTest.GetLoginAsync(expected.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(LoginDataLayer))]
        public async Task WHEN_UpdateLoginStorageAsync_is_called_THEN_login_is_updated(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var loginId = await systemUnderTest.InsertLoginStorageAsync(new LoginStorage
            {
                AccountId = 1,
                Salt = "randomSalt",
                Hash = "hashedPassword"
            });

            var updatedLogin = new LoginStorage
            {
                LoginId = loginId,
                AccountId = 1,
                Salt = "newSalt",
                Hash = "newHashedPassword"
            };

            await systemUnderTest.UpdateLoginStorageAsync(updatedLogin);

            var actual = await systemUnderTest.GetLoginAsync(updatedLogin.AccountId);

            updatedLogin.ToExpectedObject().ShouldMatch(actual);
        }

        [Theory]
        [InlineData(typeof(LoginDataLayer))]
        public async Task WHEN_GetLoginAsync_is_called_THEN_correct_login_is_returned(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var expected = new LoginStorage
            {
                AccountId = 1,
                Salt = "randomSalt",
                Hash = "hashedPassword"
            };

            var loginId = await systemUnderTest.InsertLoginStorageAsync(expected);

            expected.LoginId = loginId;

            var actual = await systemUnderTest.GetLoginAsync(expected.AccountId);

            expected.ToExpectedObject().ShouldMatch(actual);
        }
    }
}
