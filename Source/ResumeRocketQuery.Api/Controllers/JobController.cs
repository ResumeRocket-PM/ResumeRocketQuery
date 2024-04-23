using System.Net;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Services;
using System.Collections.Generic;
using System.Linq;
using Azure.Core;
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
        private readonly IJobService _jobService;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;

        public JobController(IServiceResponseBuilder serviceResponseBuilder, IJobService jobService, IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder)
        {
            this._serviceResponseBuilder = serviceResponseBuilder;
            _jobService = jobService;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
        }

        /// <summary>
        /// This retrieves the User details
        /// </summary>
        /// <returns>A User Object.</returns>
        [HttpGet]
        [Route("postings/{resumeId}")]
        public async Task<ServiceResponseGeneric<JobPostingResponse>> GetById(int resumeId)
        {
            var resumeResult = await _jobService.GetResume(resumeId);

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
                    ResumeID = resumeResult.ResumeID,
                    Status = resumeResult.Status,
                };

                return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.NotFound);
            }

            return _serviceResponseBuilder.BuildServiceResponse<JobPostingResponse>(null, HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("postings")]
        public async Task<ServiceResponseGeneric<JobPostingsResponse>> GetByAccount()
        {
            var account = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var resumeResult = await _jobService.GetResumes(account.AccountId);

            var result = resumeResult.Select(x => new JobPostingResponse
            {
                AccountID = x.AccountID,
                ApplyDate = x.ApplyDate,
                CompanyName = x.CompanyName,
                JobUrl = x.JobUrl,
                Position = x.Position,
                ResumeContent = x.ResumeContent,
                ResumeID = x.ResumeID,
                Status = x.Status,
            }).ToList();

            return _serviceResponseBuilder.BuildServiceResponse(new JobPostingsResponse(result), HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("postings")]
        public async Task<ServiceResponse> CreateJobPosting([FromForm] CreateJobRequest request)
        {
            var account = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var resultResume = new Dictionary<string, string>();

            using (var ms = new MemoryStream())
            {
                request.FormFile.CopyTo(ms);
                var fileByteArray = ms.ToArray();
                string fileBytes = Convert.ToBase64String(fileByteArray);

                resultResume.Add("FileName", request.FormFile.FileName);
                resultResume.Add("FileBytes", fileBytes);
            }

            var createRequest = JsonConvert.DeserializeObject<CreateJobPostingRequest>(request.Data);

            await _jobService.CreateJobResumeAsync(new Job
            {
                Resume = resultResume,
                JobUrl = createRequest.Url,
                AccountId = account.AccountId
            });

            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.Created);
        }
    }
}
