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
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Api.Request;

namespace ResumeRocketQuery.Api.Tests
{
    public class AccountControllerTests
    {
        private readonly RestRequestClient _restRequestClient;

        public AccountControllerTests()
        {
            _restRequestClient = new RestRequestClient();
        }

        public class GetAccount : AccountControllerTests
        {
            [Fact]
            public async Task GIVEN_jwt_is_passed_WHEN_GetAccount_is_called_THEN_user_is_able_to_access_endpoint()
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
                            HttpStatusCode = 200
                        },
                        Result = new
                        {
                            Email = email
                        }
                    };

                    var resource = $"{selfHost.Url}/api/account/{createAccountResponse.AccountId}";

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                    var actual = await _restRequestClient.SendRequest<AccountResponseBody>(resource, HttpMethod.Get, null, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }

        public class Get : AccountControllerTests
        {
            [Fact]
            public async Task GIVEN_jwt_is_passed_WHEN_GET_is_called_THEN_user_is_able_to_access_endpoint()
            {
                using (var selfHost = new WebApiStarter().Start(typeof(Startup)))
                {
                    var accountService = selfHost.ServiceProvider.GetService<IAccountService>();

                    var email = $"{Guid.NewGuid().ToString()}@testemail.com";
                    var password = "testPassword1";
                    var firstName = "john";
                    var lastName = "doe";

                    var createAccountResponse = await accountService.CreateAccountAsync(new CreateAccountRequest
                    {
                        EmailAddress = email,
                        Password = password,
                        FirstName = firstName, 
                        LastName = lastName
                    });

                    var expected = new
                    {
                        Succeeded = true,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 200
                        },
                        Result = new
                        {
                            Email = email
                        }
                    };

                    var resource = $"{selfHost.Url}/api/account/details";

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                     var actual = await _restRequestClient.SendRequest<AccountResponseBody>(resource, HttpMethod.Get, null, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }

        public class Search : AccountControllerTests
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
                            HttpStatusCode = 200
                        },
                    };

                    var resource = $"{selfHost.Url}/api/account/search";

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                    var actual = await _restRequestClient.SendRequest<List<SearchResult>>(resource, HttpMethod.Post, 
                        new SearchAccountRequest
                        {
                            ResultCount = 5,
                            SearchTerm = "John"
                        }, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);

                    Assert.Equal(5, actual.Result.Count);
                }
            }
        }


    }
}
