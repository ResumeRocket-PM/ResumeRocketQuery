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

namespace ResumeRocketQuery.Api.Tests
{
    public class LanguageControllerTests
    {
        private readonly RestRequestClient _restRequestClient;

        public LanguageControllerTests()
        {
            _restRequestClient = new RestRequestClient();
        }

        public class Get : LanguageControllerTests
        {
            [Fact]
            public async Task GIVEN_jwt_is_passed_WHEN_GET_is_called_THEN_user_is_able_to_access_endpoint()
            {
                using (var selfHost = new WebApiStarter().Start(typeof(Startup)))
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
                         Url = "https://www.metacareers.com/jobs/788246929742797/"
                     }, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }
    }
}
