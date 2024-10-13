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

namespace ResumeRocketQuery.Api.Controllers
{
    /// <summary>
    ///     This is a controller that handles the retrieval and modification of PDFs.
    /// </summary>
    [Authorize]
    [Route("api/resume")]
    public class ResumeController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeService resumeService;
        public ResumeController(IServiceResponseBuilder serviceResponseBuilder, IResumeService resumeService)
        {
            this._serviceResponseBuilder = serviceResponseBuilder;
            this.resumeService = resumeService;
        }

        /// <summary>
        ///     This retrieves a 
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpGet]
        [Route("get/{resumeId}")]
        public async Task<ServiceResponse> Get(int resumeId)
        {
            await resumeService.GetResume(resumeId);
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     This retrieves the version history of a Resume
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpGet]
        [Route("history")]
        public async Task<ServiceResponse> History(int originalResumeId)
        {
            await resumeService.GetResumeHistory(originalResumeId);
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     This updates the Resume content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpPost]
        [Route("post/{resumeId}")]
        public async Task<ServiceResponse> Post(ResumeStorage resumeObject)
        {
            await resumeService.UpdateResume(resumeObject);
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }
    }
}
