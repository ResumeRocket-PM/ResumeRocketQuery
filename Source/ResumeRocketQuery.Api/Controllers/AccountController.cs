using System.Net;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResumeRocketQuery.Api.Controllers
{
    /// <summary>
    /// This is a controller that handles the creation and modification of accounts.
    /// </summary>
    [Authorize]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly IAccountService _accountService;

        public AccountController(IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder,
            IAccountService accountService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _accountService = accountService;
        }

        /// <summary>
        /// This retrieves the User details
        /// </summary>
        /// <returns>A User Object.</returns>
        [HttpGet]
        [Route("details")]
        public async Task<ServiceResponse<AccountResponseBody>> Get()
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var accountResponse = await _accountService.GetAccountAsync(user.AccountId);

            var response = new AccountResponseBody
            {
                EmailAddress = accountResponse.EmailAddress
            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.OK);
        }
    }
}
