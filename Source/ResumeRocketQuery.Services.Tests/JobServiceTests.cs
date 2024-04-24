using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Collections.Generic;

namespace ResumeRocketQuery.Services.Tests
{
    public class JobServiceTests
    {
        private readonly IJobService _systemUnderTest;
        private readonly IAccountService _accountService;

        public JobServiceTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IJobService>();
            _accountService = serviceProvider.GetService<IAccountService>();
        }

        public class CaptureJobPostingAsync : JobServiceTests
        {
            [Fact]
            public async Task WHEN_CaptureJobPostingAsync_is_called_THEN_resume_is_persisted()
            {
                var account = await _accountService.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = $"{Guid.NewGuid().ToString()}@gmail.com",
                    Password = Guid.NewGuid().ToString()
                });

                var jobUrl =
                    "https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907";

                var resumeId = await  _systemUnderTest.CreateJobResumeAsync(new Job
                {
                    Resume = new Dictionary<string, string>(),
                    JobUrl = jobUrl,
                    AccountId = account.AccountId
                });

                var expected = new 
                {
                    JobUrl = jobUrl,
                    AccountID = account.AccountId,
                    ApplyDate = Expect.Any<DateTime>(),
                    CompanyName = Expect.Any<string>(),
                    Position = "Software Engineer",
                    ResumeContent = new Dictionary<string, string>() { { "Reccomendations", "" } },
                    ResumeID = resumeId,
                    Status = "Pending"
                };

                var actual = await _systemUnderTest.GetResume(resumeId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task WHEN_CaptureJobPostingAsync_is_called_THEN_id_is_created()
            {
                var account = await _accountService.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = $"{Guid.NewGuid().ToString()}@gmail.com",
                    Password = Guid.NewGuid().ToString()
                });

                var jobUrl =
                    "https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907";

                var resumeId = await _systemUnderTest.CreateJobResumeAsync(new Job
                {
                    Resume = new Dictionary<string, string>(),
                    JobUrl = jobUrl,
                    AccountId = account.AccountId
                });

                Assert.True(resumeId > 0);
            }
        }

        public class GetResumes : JobServiceTests
        {
            [Fact]
            public async Task WHEN_CaptureJobPostingAsync_is_called_THEN_resume_is_persisted()
            {
                var account = await _accountService.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = $"{Guid.NewGuid().ToString()}@gmail.com",
                    Password = Guid.NewGuid().ToString()
                });

                var jobUrl =
                    "https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907";

                var resumeId1 = await _systemUnderTest.CreateJobResumeAsync(new Job
                {
                    Resume = new Dictionary<string, string>(),
                    JobUrl = jobUrl,
                    AccountId = account.AccountId
                });

                var resumeId2 = await _systemUnderTest.CreateJobResumeAsync(new Job
                {
                    Resume = new Dictionary<string, string>(),
                    JobUrl = jobUrl,
                    AccountId = account.AccountId
                });

                var expected = new[]
                {
                    new
                    {
                        JobUrl = jobUrl,
                        AccountID = account.AccountId,
                        ApplyDate = Expect.Any<DateTime>(),
                        CompanyName = Expect.Any<string>(),
                        Position = "Software Engineer",
                        ResumeContent = new Dictionary<string, string> {{"Reccomendations", "" } },
                        ResumeID = resumeId1,
                        Status = "Pending"
                    },
                    new
                    {
                        JobUrl = jobUrl,
                        AccountID = account.AccountId,
                        ApplyDate = Expect.Any<DateTime>(),
                        CompanyName = Expect.Any<string>(),
                        Position = "Software Engineer",
                        ResumeContent = new Dictionary<string, string>() {{"Reccomendations", "" } },
                        ResumeID = resumeId2,
                        Status = "Pending"
                    }
                };

                var actual = await _systemUnderTest.GetResumes(account.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task update_resume_work_correctly()
            {
                var account = await _accountService.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = $"{Guid.NewGuid().ToString()}@gmail.com",
                    Password = Guid.NewGuid().ToString()
                });

                var jobUrl =
                    "https://wasatchproperty.wd1.myworkdayjobs.com/en-US/MarketStarCareers/job/MarketStar-Bulgaria---Remote/Data-Engineer_R13907";

                var resumeId1 = await _systemUnderTest.CreateJobResumeAsync(new Job
                {
                    Resume = new Dictionary<string, string>(),
                    JobUrl = jobUrl,
                    AccountId = account.AccountId
                });

                //var resumeId2 = await _systemUnderTest.CreateJobResumeAsync(new Job
                //{
                //    Resume = new Dictionary<string, string>(),
                //    JobUrl = jobUrl,
                //    AccountId = account.AccountId
                //});
                //expected.ToExpectedObject().ShouldMatch(actual);

                

                var actualOld = await _systemUnderTest.GetResume(resumeId1);
                string updateStatus = "update Status from jobServie";
                Assert.NotEqual(updateStatus, actualOld.Status);

                await _systemUnderTest.UpdateResume(resumeId1, updateStatus);
                var actualnew = await _systemUnderTest.GetResume(resumeId1);
                Assert.Equal(updateStatus, actualnew.Status);



                //expected.ToExpectedObject().ShouldMatch(actual);

                //var actualOld = await _systemUnderTest.GetResume(1);
                //string updateStatus = "update Status from jobServie";
                //Assert.NotEqual(updateStatus, actualOld.Status);

                //await _systemUnderTest.UpdateResume(1, updateStatus);
                //var actualnew = await _systemUnderTest.GetResume(1);
                //Assert.Equal(updateStatus, actualnew.Status);
            }
        }
    }
}
