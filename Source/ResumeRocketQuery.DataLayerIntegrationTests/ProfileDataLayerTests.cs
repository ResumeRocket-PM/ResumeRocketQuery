using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ResumeRocketQuery.DataLayerIntegrationTests
{
    [Rollback]
    public class ProfileDataLayerTests
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public ProfileDataLayerTests()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
        }

        private IProfileDataLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<IProfileDataLayer>();
        }

        [Theory]
        [InlineData(typeof(ProfileDataLayer))]
        public async Task WHEN_Search_State_Name_Async_is_called_THEN_results_returned_correctly_ASC(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchStateNameAsync("uta", true);

            Assert.Equal(10, results.Count);
        }

        
    }
}
