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


        [HttpGet]
        [Route("{resumeId}")]
        public async Task<ServiceResponseGeneric<string>> Get([FromRoute] int resumeId)
        {
            var result = await _resumeService.GetResume(resumeId);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        /// <summary>
        ///     This retrieves the version history of a Resume
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpGet]
        [Route("history")]
        public async Task<ServiceResponseGeneric<List<ResumeResult>>> History(int originalResumeId)
        {
            var resumeHistory = await _resumeService.GetResumeHistory(originalResumeId);
            return _serviceResponseBuilder.BuildServiceResponse(resumeHistory, HttpStatusCode.OK);
        }

        /// <summary>
        ///     This updates the Resume content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpPost]
        [Route("{resumeId}")]
        public async Task<ServiceResponse> Post(ResumeStorage resumeObject)
        {
            await _resumeService.UpdateResume(resumeObject);
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }

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

        [HttpGet]
        [Route("primary/pdf")]
        public async Task<IActionResult> GetPrimaryPdf()
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var result = await _resumeService.GetPrimaryResumePdf(user.AccountId);

            return File(result, "application/pdf", "resume.pdf");
        }

        [HttpGet]
        [Route("{resumeId}/pdf")]
        public async Task<IActionResult> GetPdf([FromRoute] int resumeId)
        {
            var result = await _resumeService.GetResumePdf(resumeId);

            return File(result, "application/pdf", "resume.pdf");
        }

        [HttpGet]
        [Route("all/{accountId}")]
        public async Task<List<ResumeResult>> GetAccountResumes([FromRoute] int accountId) {
            var result = await _resumeService.GetAccountResumes(accountId);
            return result;
        }
    }
}
