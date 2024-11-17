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
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Api.Builder;
using System.IO;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using OpenQA.Selenium.Interactions;
using static ResumeRocketQuery.DataLayer.DataLayerConstants.StoredProcedures;

namespace ResumeRocketQuery.Api.Controllers
{
    [Authorize]
    [Route("api/resume")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;
        private readonly IApplicationService _applicationService;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        
        public ResumeController(IServiceResponseBuilder serviceResponseBuilder, 
            IResumeService resumeService,
            IApplicationService applicationService,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeService = resumeService;
            _applicationService = applicationService;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
        }

        /// <summary>
        ///    Retrieves a Resume by its ID
        /// </summary>
        /// <param name="resumeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{resumeId}")]
        public async Task<ServiceResponseGeneric<string>> Get([FromRoute] int resumeId)
        {
            var result = await _resumeService.GetResume(resumeId);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("{applicationId}/suggestions")]
        public async Task<ServiceResponseGeneric<GetResumeResult>> GetPerfectResume([FromRoute] int applicationId)
        {
            var application = await _applicationService.GetApplication(applicationId);

            var resumeId = application.ResumeContentId;

            var result = await _resumeService.GetPerfectResume(resumeId, applicationId);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpPut]
        [Route("{resumeId}/suggestions/{resumeChangeId}")]
        public async Task<ServiceResponse> ApplyResumeSuggestion([FromRoute] int resumeChangeId, ResumeSuggestionsUpdateRequest resumeSuggestionsUpdateRequest)
        {
            await _resumeService.ApplyResumeSuggestion(resumeChangeId, resumeSuggestionsUpdateRequest.Accepted);

            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }


        /// <summary>
        ///     Retrieves the version history of a Resume
        /// </summary>
        /// <param name="originalResumeId"></param>
        /// <returns>A PDF Object</returns>
        [HttpGet]
        [Route("{originalResumeId}/history")]
        public async Task<ServiceResponseGeneric<List<ResumesResponseBody>>> History([FromRoute] int originalResumeId)
        {
            var resumeHistory = await _resumeService.GetResumeHistory(originalResumeId);

            var result = resumeHistory.Select(x => new ResumesResponseBody
            {
                ResumeId = x.ResumeId,
                InsertDate = x.InsertDate,
                OriginalResumeID = x.OriginalResumeID,
                UpdateDate = x.UpdateDate
            }).ToList();

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        ///     Updates a Resume's content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpPost]
        [Route("{resumeId}")]
        public async Task<ServiceResponse> Post(ResumeStorage resumeObject)
        {
            await _resumeService.UpdateResume(resumeObject);
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Creates the primary Resume for the account
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("primary")]
        public async Task<ServiceResponse> CreatePrimary([FromForm] IFormFile file)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var resultResume = new Dictionary<string, string>();

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileByteArray = ms.ToArray();
                string fileBytes = Convert.ToBase64String(fileByteArray);

                resultResume.Add("FileName", file.FileName);
                resultResume.Add("FileBytes", fileBytes);
            }

            await _resumeService.CreatePrimaryResume(new ResumeRequest
            {
                AccountId = user.AccountId,
                Pdf = resultResume
            });

            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.Created);
        }

        /// <summary>
        ///     Retrieves the primary Resume for the account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("primary/pdf")]
        public async Task<IActionResult> GetPrimaryPdf()
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var result = await _resumeService.GetPrimaryResumePdf(user.AccountId);

            return File(result, "application/pdf", "resume.pdf");
        }

        /// <summary>
        ///    Retrieves a Resume for the account by ID as a PDF
        /// </summary>
        /// <param name="resumeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{resumeId}/pdf")]
        public async Task<IActionResult> GetPdf([FromRoute] int resumeId)
        {
            var result = await _resumeService.GetResumePdf(resumeId);

            return File(result, "application/pdf", "resume.pdf");
        }

        /// <summary>
        ///   Retrieves all Resumes for the account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<List<ResumesResponseBody>> GetAccountResumes() {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);
            var accountId = user.AccountId;
            var result = await _resumeService.GetAccountResumes(accountId);

            return result.Select(x => new ResumesResponseBody
            {
                ResumeId = x.ResumeId,
                InsertDate = x.InsertDate,
                OriginalResumeID = x.OriginalResumeID,
                UpdateDate = x.UpdateDate
            }).ToList();
        }

        /// <summary>
        ///    Creates a Resume for the account
        /// </summary>
        /// <param name="file"></param>
        /// <param name="createResumeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<ServiceResponseGeneric<CreateResumeResponse>> CreateResume([FromForm] IFormFile file, CreateResumeRequest createResumeRequest)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var resultResume = new Dictionary<string, string>();

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileByteArray = ms.ToArray();
                string fileBytes = Convert.ToBase64String(fileByteArray);

                resultResume.Add("FileName", file.FileName);
                resultResume.Add("FileBytes", fileBytes);
            }

            var resumeId = await _resumeService.CreateResume(new ResumeRequest
            {
                AccountId = user.AccountId,
                Pdf = resultResume,
                OriginalResume = createResumeRequest.OriginalResume,
            });

            var response = new CreateResumeResponse
            {
                ResumeId = resumeId.ToString()
            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.Created);
        }


        /// <summary>
        ///    Adds a resume to version history of the original resume
        /// </summary>
        /// <param name="file"></param>
        /// <param name="originalResumeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{originalResumeId}/addToVersionHistory")]
        public async Task<ServiceResponseGeneric<CreateResumeResponse>> AddToVersionHistory(CreateResumeRequest createResumeRequest, [FromRoute] int originalResumeId)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var resumeHtmlString = createResumeRequest.ResumeHtmlString;
            var file = await _resumeService.GetResumePdfFromHtml(resumeHtmlString);

            var resultResume = new Dictionary<string, string>();

            using (var ms = new MemoryStream())
            {
                ms.Write(file, 0, file.Length); 

                var fileBytes = Convert.ToBase64String(ms.ToArray());

                resultResume.Add("FileName", "resumeVersionFor" + originalResumeId);
                resultResume.Add("FileBytes", fileBytes);
            }

            var resumeId = await _resumeService.CreateResume(new ResumeRequest
            {
                AccountId = user.AccountId,
                Pdf = resultResume,
                OriginalResumeID = originalResumeId,
            });

            var response = new CreateResumeResponse
            {
                ResumeId = resumeId.ToString()
            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.Created);
        }
    }
}
