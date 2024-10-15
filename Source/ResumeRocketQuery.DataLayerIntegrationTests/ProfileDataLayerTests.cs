using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
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

            var results = await systemUnderTest.SearchStateNameAsync("ut", true);

            Assert.Equal(5, results.Count);
        }

        [Theory]
        [InlineData(typeof(ProfileDataLayer))]
        public async Task WHEN_Search_State_Name_Async_is_called_THEN_results_returned_correctly_DESC(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchStateNameAsync("ut", false);

            Assert.Equal(5, results.Count);
        }

        [Theory]
        [InlineData(typeof(ProfileDataLayer))]
        public async Task WHEN_Search_Univ_Name_Async_is_called_THEN_results_returned_correctly_ASC(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchUniversityNameAsync("ut", true);

            Assert.Equal(157, results.Count);
        }

        [Theory]
        [InlineData(typeof(ProfileDataLayer))]
        public async Task WHEN_Search_Univ_Name_Async_is_called_THEN_results_returned_correctly_DESC(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchUniversityNameAsync("ut", false);

            Assert.Equal(157, results.Count);
        }

        [Theory]
        [InlineData(typeof(ProfileDataLayer))]
        public async Task WHEN_Search_Career_Name_Async_is_called_THEN_results_returned_correctly_ASC(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchCareerNameAsync("com", true);

            Assert.Equal(4, results.Count);
            Assert.Equal("Computer and Information Systems ManagersMechanical Engineers", results[0].CareerName);

        }

        [Theory]
        [InlineData(typeof(ProfileDataLayer))]
        public async Task WHEN_Search_Career_Name_Async_is_called_THEN_results_returned_correctly_DESC(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchCareerNameAsync("com", false);

            Assert.Equal(4, results.Count);
            Assert.Equal("Property, Real Estate, and Community Association Managers", results[0].CareerName);

        }

        [Theory]
        [InlineData(typeof(ProfileDataLayer))]
        public async Task WHEN_Search_Major_Name_Async_is_called_THEN_results_returned_correctly_ASC(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchMajorNameAsync("com", true);

            Assert.Equal(18, results.Count);
            Assert.Equal("Commercial Art and Graphic Design", results[0].MajorName);

        }

        [Theory]
        [InlineData(typeof(ProfileDataLayer))]
        public async Task WHEN_Search_Major_Name_Async_is_called_THEN_results_returned_correctly_DESC(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var results = await systemUnderTest.SearchMajorNameAsync("com", false);

            Assert.Equal(18, results.Count);
            Assert.Equal("Science and Computer Teacher Education", results[0].MajorName);

        }


    }
}
