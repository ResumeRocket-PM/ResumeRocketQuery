using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Api.Controllers
{
    [Authorize]
    [Route("api/extension")]
    public class ExtensionController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly IExtensionService _extensionService;

        public ExtensionController(IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder,
            IExtensionService extensionService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _extensionService = extensionService;
        }

        [HttpPost]
        [Route("webpage/isapplication")]
        public async Task<ServiceResponseGeneric<bool>> Status([FromBody] ExtensionRequest extensionRequest)
        {
            var result = await _extensionService.IsJobApplication(extensionRequest.Html);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }
    }
}
