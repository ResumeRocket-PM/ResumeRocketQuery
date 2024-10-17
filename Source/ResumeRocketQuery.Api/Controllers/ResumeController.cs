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
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        
        public ResumeController(IServiceResponseBuilder serviceResponseBuilder, 
            IResumeService resumeService,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeService = resumeService;
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

        /// <summary>
        ///     Retrieves the version history of a Resume
        /// </summary>
        /// <param name="originalResumeId"></param>
        /// <returns>A PDF Object</returns>
        [HttpGet]
        [Route("{originalResumeId}/history")]
        public async Task<ServiceResponseGeneric<List<ResumeResult>>> History([FromRoute] int originalResumeId)
        {
            var resumeHistory = await _resumeService.GetResumeHistory(originalResumeId);
            return _serviceResponseBuilder.BuildServiceResponse(resumeHistory, HttpStatusCode.OK);
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
        public async Task<List<ResumeResult>> GetAccountResumes() {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);
            var accountId = user.AccountId;
            var result = await _resumeService.GetAccountResumes(accountId);
            return result;
        }
        
        /// <summary>
        ///    Creates a Resume for the account
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<ServiceResponseGeneric<CreateResumeResponse>> CreateResume([FromForm] IFormFile file)
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
                Pdf = resultResume
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
        /// <param name="resumeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{originalResumeId}/addToVersionHistory/{resumeId}")]
        public async Task<ServiceResponseGeneric<CreateResumeResponse>> AddToVersionHistory([FromForm] IFormFile file, [FromRoute] int originalResumeId, [FromRoute] int resumeId)
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

            await _resumeService.CreateResume(new ResumeRequest
            {
                AccountId = user.AccountId,
                Pdf = resultResume,
                OriginalResumeID = originalResumeId,
                ResumeId = resumeId
            });

            var response = new CreateResumeResponse
            {
                ResumeId = resumeId.ToString()
            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.Created);
        }
    }
}
