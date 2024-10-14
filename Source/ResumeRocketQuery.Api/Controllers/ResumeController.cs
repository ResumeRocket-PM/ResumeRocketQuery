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
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeService _resumeService;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        public ResumeController(IServiceResponseBuilder serviceResponseBuilder, 
            IResumeService resumeService,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeService = resumeService;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
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
        public async Task<IActionResult> Get()
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var result = await _resumeService.GetPrimaryResumePdf(user.AccountId);

            return File(result, "application/pdf", "resume.pdf");
        }


        [HttpGet]
        [Route("primary")]
        public async Task<ServiceResponseGeneric<string>> GetPrimaryPdf([FromBody] string resume, string jobUrl)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var result = await _resumeService.GetPrimaryResume(user.AccountId);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }
    }
}
