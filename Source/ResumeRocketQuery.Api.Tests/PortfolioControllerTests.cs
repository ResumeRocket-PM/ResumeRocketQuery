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
using ResumeRocketQuery.Domain.Api.Request;
using Microsoft.Extensions.Configuration;

namespace ResumeRocketQuery.Api.Tests
{
    public class PortfolioControllerTests
    {
        private readonly RestRequestClient _restRequestClient;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _testConfiguration;

        public PortfolioControllerTests()
        {
            _restRequestClient = new RestRequestClient();

            _testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:ResumeRocketQueryDatabaseConnectionString", "Server=localhost; Database=ResumeRocketTest; Trusted_Connection=True; TrustServerCertificate=True;" }
                })
                .Build();
        }

        public class Post : PortfolioControllerTests
        {
            [Fact]
            public async Task GIVEN_jwt_is_passed_WHEN_POST_is_called_THEN_user_is_able_to_access_endpoint()
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

                    var resource = $"{selfHost.Url}/api/portfolio";

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                    var actual = await _restRequestClient.SendRequest<PortfolioResponseBody>(resource, HttpMethod.Post, new PortfolioRequestBody
                    {
                        Content = Guid.NewGuid().ToString()
                    }, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }

        public class Get : PortfolioControllerTests
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

                    var portfolioService = selfHost.ServiceProvider.GetService<IPortfolioService>();

                    var configuration = Guid.NewGuid().ToString();

                    await portfolioService.CreatePortfolio(new Portfolio
                    {
                        AccountId = createAccountResponse.AccountId,
                        Configuration = configuration
                    });

                    var expected = new
                    {
                        Succeeded = true,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 200
                        },
                        Result = new PortfolioResponseBody
                        {
                            Content = configuration
                        }
                    };

                    var resource = $"{selfHost.Url}/api/portfolio/details";

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
                    };

                     var actual = await _restRequestClient.SendRequest<PortfolioResponseBody>(resource, HttpMethod.Get, null, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }
    }
}
