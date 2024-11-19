using System.Net;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using Newtonsoft.Json;

namespace ResumeRocketQuery.Api.Controllers
{
    /// <summary>
    /// This is a controller that handles the creation and modification of accounts.
    /// </summary>
    [Authorize]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IApplicationService _applicationService;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;

        public JobController(
            IServiceResponseBuilder serviceResponseBuilder, 
            IApplicationService applicationService,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder
            )
        {
            this._serviceResponseBuilder = serviceResponseBuilder;
            _applicationService = applicationService;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
        }

        /// <summary>
        /// This retrieves the User details
        /// </summary>
        /// <returns>A User Object.</returns>
        [HttpGet]
        [Route("postings/{applicationId}")]
        public async Task<ServiceResponseGeneric<JobPostingResponse>> GetById(int applicationId)
        {
            var resumeResult = await _applicationService.GetApplication(applicationId);

            if (resumeResult != null)
            {
                var result = new JobPostingResponse
                {
                    AccountID = resumeResult.AccountID,
                    ApplyDate = resumeResult.ApplyDate,
                    CompanyName = resumeResult.CompanyName,
                    JobUrl = resumeResult.JobUrl,
                    Position = resumeResult.Position,
                    ResumeContent = resumeResult.ResumeContent,
                    Status = resumeResult.Status,
                    ResumeContentId = resumeResult.ResumeContentId
                };

                return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
            }

            return _serviceResponseBuilder.BuildServiceResponse<JobPostingResponse>(null, HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("postings")]
        public async Task<ServiceResponseGeneric<JobPostingsResponse>> GetByAccount()
        {
            var account = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var resumeResult = await _applicationService.GetJobPostings(account.AccountId);

            var result = resumeResult.Select(x => new JobPostingResponse
            {
                ApplicationId = x.ApplicationId,
                AccountID = x.AccountID,
                ApplyDate = x.ApplyDate,
                CompanyName = x.CompanyName,
                JobUrl = x.JobUrl,
                Position = x.Position,
                //ResumeContent = x.ResumeContent,
                ResumeContentId = x.ResumeContentId,
                Status = x.Status,
            }).ToList();

            return _serviceResponseBuilder.BuildServiceResponse(new JobPostingsResponse(result), HttpStatusCode.OK);
        }

        [HttpPut]
        [Route("postings/{resumeId}")]
        public async Task<ServiceResponse> UpdateStaus(int resumeId, string status)
        {
            await _applicationService.UpdateApplication(resumeId, status);

            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("extension/postings/{resumeId}")]
        public async Task<ServiceResponseGeneric<int>> CreateJobPosting([FromBody] CreateApplicationRequest applicationRequest, [FromRoute] int resumeId)
        {
            var account = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var applicationId = await _applicationService.CreateJobAsync(new ApplicationRequest
            {
                JobHtml = applicationRequest.Html,
                JobUrl = applicationRequest.Url,
                AccountId = account.AccountId,
                ResumeId = resumeId
            });

            //string resumeHtml = await _resumeService.GetResume(resumeId);

            //await _applicationService.CreateJobResumeFromHtmlAsync(account.AccountId, applicationRequest.Url, resumeHtml);

            return _serviceResponseBuilder.BuildServiceResponse(applicationId, HttpStatusCode.Created);
        }

        [HttpPost]
        [Route("postings")]
        public async Task<ServiceResponse> CreateJobPosting([FromForm] IFormFile file, [FromForm] string data)
        {
            var account = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var resultResume = new Dictionary<string, string>();

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileByteArray = ms.ToArray();
                string fileBytes = Convert.ToBase64String(fileByteArray);

                resultResume.Add("FileName", file.FileName);
                resultResume.Add("FileBytes", fileBytes);
            }

            var createRequest = JsonConvert.DeserializeObject<CreateJobPostingRequest>(data);

            await _applicationService.CreateJobResumeAsync(new Job
            {
                Resume = resultResume,
                JobUrl = createRequest.Url,
                AccountId = account.AccountId
            });

            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.Created);
        }
    }
}
