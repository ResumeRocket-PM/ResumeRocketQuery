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
        private readonly IJobScraper jobScraper;
        private readonly IJobService jobService;
        public ResumeController(IServiceResponseBuilder serviceResponseBuilder, IJobScraper jobScraper)
        {
            this._serviceResponseBuilder = serviceResponseBuilder;
            this.jobScraper = jobScraper;
            this.jobService = jobService;
        }

        /// <summary>
        ///     This retrieves the PDF content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpGet]
        [Route("get")]
        public async Task<ServiceResponse> Get(string request)
        {
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     This retrieves the PDF content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpGet]
        [Route("history")]
        public async Task<ServiceResponse> History(int originalResumeId)
        {
            await jobService.GetResumeHistory(originalResumeId);
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     This updates the PDF content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpPost]
        [Route("post")]
        public async Task<ServiceResponse> Post([FromBody] string resume, string jobUrl)
        {
            await jobScraper.ScrapeJobPosting(jobUrl);
            // Call to Arthur's class to save to table
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }
    }
}
