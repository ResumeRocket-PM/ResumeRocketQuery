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
using Microsoft.Extensions.Configuration;

namespace ResumeRocketQuery.Api.Tests
{
    public class AuthenticationControllerTests
    {
        private readonly RestRequestClient _restRequestClient;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _testConfiguration;


        public AuthenticationControllerTests()
        {
            _restRequestClient = new RestRequestClient();

            _testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:ResumeRocketQueryDatabaseConnectionString", "Server=localhost; Database=ResumeRocketTest; Trusted_Connection=True; TrustServerCertificate=True;" }
                })
                .Build();
        }

        public class Authenticate : AuthenticationControllerTests
        {
            [Fact]
            public async Task GIVEN_invalid_user_WHEN_Authenticate_is_called_THEN_user_is_not_returned_jwt()
            {
                using (var selfHost = new WebApiStarter(_testConfiguration).Start(typeof(Startup)))
                {
                    var authenticationRequestBody = new AuthenticationRequestBody
                    {
                        EmailAddress = Guid.NewGuid().ToString(),
                        Password = Guid.NewGuid().ToString()
                    };

                    var expected = new
                    {
                        Succeeded = true,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 200
                        },
                        Result = new
                        {
                            IsAuthenticated = false,
                            JsonWebToken = Expect.Null()
                        }
                    };

                    var actual = await _restRequestClient.SendRequest<AuthenticateAccountResponse>($"{selfHost.Url}/api/authenticate", HttpMethod.Post, authenticationRequestBody);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }

            [Fact]
            public async Task GIVEN_valid_user_WHEN_Authenticate_is_called_THEN_user_is_returned_jwt()
            {
                using (var selfHost = new WebApiStarter(_testConfiguration).Start(typeof(Startup)))
                {
                    var accountService = selfHost.ServiceProvider.GetService<IAccountService>();

                    var email = $"{Guid.NewGuid().ToString()}@testemail.com";
                    var password = "testPassword1";

                    await accountService.CreateAccountAsync(new CreateAccountRequest
                    {
                        EmailAddress = email,
                        Password = password
                    });

                    var authenticationRequestBody = new AuthenticationRequestBody
                    {
                        EmailAddress = email,
                        Password = password
                    };

                    var expected = new
                    {
                        Succeeded = true,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 200
                        },
                        Result = new
                        {
                            IsAuthenticated = true,
                            JsonWebToken = Expect.NotNull()
                        }
                    };

                    var actual = await _restRequestClient.SendRequest<AuthenticateAccountResponse>($"{selfHost.Url}/api/authenticate", HttpMethod.Post, authenticationRequestBody);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }

        public class Post : AuthenticationControllerTests
        {
            [Fact]
            public async Task WHEN_POST_is_called_THEN_user_is_returned_jwt()
            {
                using (var selfHost = new WebApiStarter(_testConfiguration).Start(typeof(Startup)))
                {
                    var email = $"{Guid.NewGuid().ToString()}@testemail.com";
                    var password = "testPassword1";

                    var authenticationRequestBody = new AccountRequestBody
                    {
                        EmailAddress = email,
                        Password = password,
                        FirstName = "John",
                        LastName = "Doe"
                    };

                    var expected = new
                    {
                        Succeeded = true,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 201
                        },
                        Result = new
                        {
                            IsAuthenticated = true,
                            JsonWebToken = Expect.NotNull()
                        }
                    };

                    var actual = await _restRequestClient.SendRequest<AuthenticateAccountResponse>($"{selfHost.Url}/api/account", HttpMethod.Post, authenticationRequestBody);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }

            [Fact]
            public async Task GIVEN_invalid_data_WHEN_POST_is_called_THEN_validation_error_returned()
            {
                using (var selfHost = new WebApiStarter(_testConfiguration).Start(typeof(Startup)))
                {
                    var email = $"{Guid.NewGuid().ToString()}";
                    var password = "testPassword1";

                    var authenticationRequestBody = new AccountRequestBody
                    {
                        EmailAddress = email,
                        Password = password,
                        FirstName = "John",
                        LastName = "Doe"
                    };

                    var expected = new
                    {
                        Succeeded = false,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 400,
                            Exception = Expect.Null(),
                            ValidationErrors = new []
                            {
                                new
                                {
                                    Property = "EmailAddress",
                                    Message = @"The field EmailAddress must match the regular expression '^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$'."
                                }
                            }
                        },
                        Result = Expect.Null()
                    };

                    var actual = await _restRequestClient.SendRequest<AuthenticateAccountResponse>($"{selfHost.Url}/api/account", HttpMethod.Post, authenticationRequestBody);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }

            [Fact]
            public async Task GIVEN_invalid_length_password_WHEN_POST_is_called_THEN_validation_error_returned()
            {
                using (var selfHost = new WebApiStarter(_testConfiguration).Start(typeof(Startup)))
                {
                    var email = $"{Guid.NewGuid().ToString()}@testemail.com";
                    var password = "12345";

                    var authenticationRequestBody = new AccountRequestBody
                    {
                        EmailAddress = email,
                        Password = password,
                        FirstName = "John",
                        LastName = "Doe"
                    };

                    var expected = new
                    {
                        Succeeded = false,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 400,
                            Exception = Expect.Null(),
                            ValidationErrors = new[]
                            {
                                new
                                {
                                    Property = "Password",
                                    Message = @"The field Password must be a string or array type with a minimum length of '6'."
                                }
                            }
                        },
                        Result = Expect.Null()
                    };

                    var actual = await _restRequestClient.SendRequest<AuthenticateAccountResponse>($"{selfHost.Url}/api/account", HttpMethod.Post, authenticationRequestBody);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }
    }
}
