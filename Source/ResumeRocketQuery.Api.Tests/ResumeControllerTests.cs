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
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Services;

namespace ResumeRocketQuery.Api.Tests
{
    public class ResumeControllerTests
    {
        private readonly RestRequestClient _restRequestClient;

        public ResumeControllerTests()
        {
            _restRequestClient = new RestRequestClient();
        }


        public class ApplyResumeChanges : ResumeControllerTests
        {
            [Fact]
            public async Task GIVEN_application_id_WHEN_ApplyResumeSuggestions_is_called_THEN_suggestion_statuses_are_updated_successfully()
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
                    var applicationDataLayer = selfHost.ServiceProvider.GetService<IApplicationDataLayer>();

                    var suggestedChangeId = Guid.NewGuid().ToString();

                    var resumeId = await resumeDataLayer.InsertResumeAsync(new ResumeStorage
                    {
                        AccountId = createAccountResponse.AccountId,
                        Resume = $"<div id=\"{suggestedChangeId}\">Sample Resume Text</div>",
                        InsertDate = DateTime.Today,
                        UpdateDate = DateTime.Today,
                    });

                    var resumeChangeId = await resumeDataLayer.InsertResumeChangeAsync(new ResumeChangesStorage
                    {
                        ResumeId = resumeId,
                        Accepted = false,
                        ExplanationString = "Because it sounds nicer",
                        HtmlID = suggestedChangeId,
                        ModifiedText = "Professional Job Engineer",
                        OriginalText = "Sample Resume Text",
                    });

                    var applicationId = await applicationDataLayer.InsertApplicationAsync(new ApplicationStorage
                    {
                        AccountId = createAccountResponse.AccountId,
                        ApplyDate = DateTime.UtcNow,
                        Status = "Pending",
                        Position = "Software Engineer",
                        CompanyName = "Tech Corp",
                        JobPostingUrl = Guid.NewGuid().ToString(),
                    });

                    var headers = new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {createAccountResponse.JsonWebToken}" }
                    };

                    var expected = new
                    {
                        Succeeded = true,
                        ResponseMetadata = new
                        {
                            HttpStatusCode = 200
                        }
                    };

                    var actual = await _restRequestClient.SendRequest<object>(
                        $"{selfHost.Url}/api/resume/{applicationId}/suggestions",
                        HttpMethod.Put,
                        new ResumeSuggestionsUpdateRequest
                        {
                            SuggestionStatuses = new List<SuggestionStatus>
                            {
                        new SuggestionStatus { ResumeChangeId = resumeChangeId, IsApplied = true }
                            }
                        },
                        headers
                    );

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
        }

        public class GetPerfectResume : ResumeControllerTests
        {
            [Fact]
            public async Task GIVEN_resume_change_id_WHEN_ApplyResumeSuggestion_is_called_THEN_suggestion_is_applied_successfully()
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
                    var applicationDataLayer = selfHost.ServiceProvider.GetService<IApplicationDataLayer>();

                    var suggestedChangeId = Guid.NewGuid().ToString();

                    var resumeId = await resumeDataLayer.InsertResumeAsync(new ResumeStorage
                    {
                        AccountId = createAccountResponse.AccountId,
                        Resume = $"<div id=\"{suggestedChangeId}\">Sample Resume Text</div>",
                        InsertDate = DateTime.Today,
                        UpdateDate = DateTime.Today,
                    });

                    var applicationId = await applicationDataLayer.InsertApplicationAsync(new ApplicationStorage
                    {
                        AccountId = createAccountResponse.AccountId,
                        ResumeId = resumeId,
                        ApplyDate = DateTime.UtcNow,
                        Status = "Pending",
                        Position = "Software Engineer",
                        CompanyName = "Tech Corp",
                        JobPostingUrl = Guid.NewGuid().ToString(),
                    });

                    var resumeChangeId = await resumeDataLayer.InsertResumeChangeAsync(new ResumeChangesStorage
                    {
                        ResumeId = resumeId,
                        Accepted = true,
                        ExplanationString = "Because it sounds nicer",
                        HtmlID = suggestedChangeId,
                        ModifiedText = "Professional Job Engineer",
                        OriginalText = "Sample Resume Text",
                        ApplicationId = applicationId
                    });

                    var headers = new Dictionary<string, string>
                    {
                        {"Authorization", $"Bearer {createAccountResponse.JsonWebToken}"}
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
                            ResumeHTML = $"<div id=\"{suggestedChangeId}\">Sample Resume Text</div>",
                            ResumeId = resumeId,
                            ResumeSuggestions = new[]
                            {
                                new
                                {
                                    ResumeChangeId = resumeChangeId,
                                    Accepted = true,
                                    ExplanationString = "Because it sounds nicer",
                                    HtmlID = (string)null,
                                    ModifiedText = "Professional Job Engineer",
                                    OriginalText = "Sample Resume Text",
                                    ApplicationId = applicationId
                                }
                            }
                        }
                    };

                    var actual = await _restRequestClient.SendRequest<GetResumeResult>($"{selfHost.Url}/api/resume/{applicationId}/suggestions", HttpMethod.Get, null, headers);

                    expected.ToExpectedObject().ShouldMatch(actual);
                }
            }
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
