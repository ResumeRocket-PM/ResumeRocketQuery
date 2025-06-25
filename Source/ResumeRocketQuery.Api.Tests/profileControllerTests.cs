using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.Api.Tests.Helpers;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Api.Request;
using Microsoft.Extensions.Configuration;

namespace ResumeRocketQuery.Api.Tests
{
    public class ProfileControllerTests
    {
        private readonly RestRequestClient _restRequestClient;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _testConfiguration;

        public ProfileControllerTests()
        {
            _restRequestClient = new RestRequestClient();

            _testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:ResumeRocketQueryDatabaseConnectionString", "Server=localhost; Database=ResumeRocketTest; Trusted_Connection=True; TrustServerCertificate=True;" }
                })
                .Build();
        }

        public class Get : ProfileControllerTests
        {
            [Fact]
            public async Task Test_profileController_Get_univeristyName()
            {
                using (var selfHost = new WebApiStarter(_testConfiguration).Start(typeof(Startup)))
                {
                    var accountService = selfHost.ServiceProvider.GetService<IAccountService>();

                    var email = $"{Guid.NewGuid().ToString()}@testemail.com";
                    var password = "testPassword1";

                    var createAccountResponse = await accountService.CreateAccountAsync(new CreateAccountRequest
                    {
                        EmailAddress = email,
                        Password = password
                    });

                    var expected = new
                    {
                        Succeeded = true,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 201
                        },
                    };

                    var profileService = selfHost.ServiceProvider.GetService<IProfileService>();
                    string uname = "ut";
                    bool orderType = true;
                    var resource = $"{selfHost.Url}/api/profile/UniversityName/{uname}/{orderType}";
                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                    //var headers = new Dictionary<string, string>
                    //{
                    //    {uname, $"true"}
                    //};
                    //var actual = await _restRequestClient.SendRequest<Dictionary<string, List<string>>>(resource, HttpMethod.Get, null, headers);
                    var actual = await _restRequestClient.SendRequest<List<string>>(resource, HttpMethod.Get, null, headers);
                    var actual_result = actual.Result;
                    Assert.Equal(157, actual_result.Count);
                    Assert.Equal("Air Force Institute of Technology                                               ", actual_result[0]);
                }
            }
        }

    }
}
