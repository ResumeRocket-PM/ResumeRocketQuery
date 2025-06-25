using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Api.Tests.Helpers;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace ResumeRocketQuery.Api.Tests
{
    [Collection("External")]
    public class LanguageControllerTests
    {
        private readonly RestRequestClient _restRequestClient;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _testConfiguration;


        public LanguageControllerTests()
        {
            _restRequestClient = new RestRequestClient();

            _testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:ResumeRocketQueryDatabaseConnectionString", "Server=localhost; Database=ResumeRocketTest; Trusted_Connection=True; TrustServerCertificate=True;" }
                })
                .Build();
        }

        public class Get : LanguageControllerTests
        {
            [Fact]
            public async Task GIVEN_jwt_is_passed_WHEN_GET_is_called_THEN_user_is_able_to_access_endpoint()
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
                        Result = new
                        {
                            Description = Expect.Any<string>(),
                            Keywords = Expect.Any<List<string>>(),
                            Perks = Expect.Any<List<string>>(),
                            Requirements = Expect.Any<List<string>>(),
                            Title = Expect.Any<string>()
                        }
                    };

                    var resource = $"{selfHost.Url}/api/language/jobposting";

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                     var actual = await _restRequestClient.SendRequest<CreateJobPostingResponse>(resource, HttpMethod.Post, new CreateJobPostingRequest
                     {
                         Url = "https://www.metacareers.com/jobs/1604078213531024/"
                     }, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }
    }
}
