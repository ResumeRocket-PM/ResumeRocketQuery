using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class ProfileServiceTest
    {
        private readonly IProfileService _systemUnderTest;

        public ProfileServiceTest()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IProfileService>();
        }

        public class CreateAccountAsync : ProfileServiceTest
        {
            [Fact]
            public async Task provide_The_Correct_search_result_from_university()
            {
                var result = await _systemUnderTest.GetUniversity("ut", true);
                Assert.Equal(157, result.Count);
                Assert.Equal("Air Force Institute of Technology                                               ", result[0].UniversityName);

                //Assert.NotNull(result);
            }
        }

    }
}
