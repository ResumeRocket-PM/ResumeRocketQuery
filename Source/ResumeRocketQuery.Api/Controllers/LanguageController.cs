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

namespace ResumeRocketQuery.Api.Controllers
{
    /// <summary>
    /// This is a controller that handles the creation and modification of accounts.
    /// </summary>
    [Authorize]
    [Route("api/language")]
    public class LanguageController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly ILanguageService languageService;

        public LanguageController(IServiceResponseBuilder serviceResponseBuilder, ILanguageService languageService)
        {
            this._serviceResponseBuilder = serviceResponseBuilder;
            this.languageService = languageService;
        }

        /// <summary>
        /// This retrieves the User details
        /// </summary>
        /// <returns>A User Object.</returns>
        [HttpPost]
        [Route("jobposting")]
        public async Task<ServiceResponse<CreateJobPostingResponse>> Post([FromBody]CreateJobPostingRequest request)
        {
            var jobPostingResult = await languageService.CaptureJobPostingAsync(request.Url);

            var result = new CreateJobPostingResponse
            {
                DatePosted = jobPostingResult.DatePosted,
                Description = jobPostingResult.Description,
                Keywords    = jobPostingResult.Keywords,
                Perks = jobPostingResult.Perks, 
                Requirements = jobPostingResult.Requirements,
                Title = jobPostingResult.Title
            };

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.Created);
        }
    }
}
