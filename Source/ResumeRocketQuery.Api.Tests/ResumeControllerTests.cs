using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Api.Tests.Helpers;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;
using ResumeRocketQuery.Domain.Api;
using Microsoft.Identity.Client;
using ResumeRocketQuery.Domain.DataLayer;

namespace ResumeRocketQuery.Api.Tests
{
    public class ResumeControllerTests
    {
        private readonly RestRequestClient _restRequestClient;

        public ResumeControllerTests()
        {
            _restRequestClient = new RestRequestClient();
        }

        public class Get : ResumeControllerTests
        {
            [Fact]
            public async Task GIVEN_jwt_is_passed_WHEN_GET_is_called_THEN_user_is_able_to_access_resume()
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

                    var resumeDataLayer = selfHost.ServiceProvider.GetService<IResumeDataLayer>();

                    var resumeId = await resumeDataLayer.InsertResumeAsync(new ResumeStorage
                    {
                        AccountId = createAccountResponse.AccountId,
                        Resume = "Sample Resume Text",
                        InsertDate = DateTime.Today,
                        UpdateDate = DateTime.Today,
                    });

                    var expected = new
                    {
                        Succeeded = true,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 200
                        },
                        Result = "Sample Resume Text"
                    };

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                    var actual = await _restRequestClient.SendRequest<string>($"{selfHost.Url}/api/resume/{resumeId}", HttpMethod.Get, null, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }

    }
}
