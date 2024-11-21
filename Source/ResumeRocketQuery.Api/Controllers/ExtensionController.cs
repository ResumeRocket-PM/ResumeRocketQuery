using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Services;
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
        private readonly IAccountService _accountService;

        public ExtensionController(IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder,
            IExtensionService extensionService,
            IAccountService accountService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _extensionService = extensionService;
            _accountService = accountService;
        }

        [HttpPost]
        [Route("webpage/isapplication")]
        public async Task<ServiceResponseGeneric<bool>> Status([FromBody] ExtensionRequest extensionRequest)
        {
            var result = await _extensionService.IsJobApplication(extensionRequest.Html);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("webpage/embedPath")]
        public async Task<ServiceResponseGeneric<string>> GetEmbeddedXPath([FromBody] ExtensionRequest extensionRequest)
        {
            var result = await _extensionService.CreateHtmlQueryForEmbeddingButton(extensionRequest.Url, extensionRequest.Html);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("webpage/applybutton")]
        public async Task<ServiceResponseGeneric<string>> GetApplyButtonXPath([FromBody] ExtensionRequest extensionRequest)
        {
            var result = _extensionService.GenerateApplyButtonXPath(extensionRequest.Url);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("webpage/probability")]
        public async Task<ServiceResponseGeneric<decimal>> GenerateProbability([FromBody] ExtensionRequest extensionRequest)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var account = await _accountService.GetAccountAsync(user.AccountId);

            var result = await _extensionService.GenerateProbabilityMatchAgainstJob(extensionRequest.Html, account.PrimaryResumeId.Value);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }
    }
}
