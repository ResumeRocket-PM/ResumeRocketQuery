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
    public class JobControllerTests
    {
        private readonly RestRequestClient _restRequestClient;

        public JobControllerTests()
        {
            _restRequestClient = new RestRequestClient();
        }

        public class Get : JobControllerTests
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
                    };

                    var resource = $"{selfHost.Url}/api/job/postings";

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                     var actual = await _restRequestClient.SendRequest<CreateJobPostingResponse>(resource, 
                         HttpMethod.Post, 
                         new CreateJobPostingRequest
                         {
                             Url = "https://www.metacareers.com/resume/?req=a1K2K000007p93VUAQ"
                         }, headers, fileUpload: "./Samples/Tyler DeBruin Resume.pdf");

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }
    }
}
