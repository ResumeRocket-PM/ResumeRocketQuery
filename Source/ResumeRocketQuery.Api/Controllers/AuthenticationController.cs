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
    [AllowAnonymous]
    [Route("api")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountService _accountService;

        public AuthenticationController(IServiceResponseBuilder serviceResponseBuilder,
            IAuthenticationService resumeRocketQueryService, 
            IAccountService accountService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _authenticationService = resumeRocketQueryService;
            _accountService = accountService;
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<ServiceResponseGeneric<AuthenticationResponseBody>> Authenticate([FromBody] AuthenticationRequestBody authenticationRequestBody)
        {
            var authenticationResponse = await _authenticationService.AuthenticateAccountAsync(new AuthenticateAccountRequest
            {
                EmailAddress = authenticationRequestBody.EmailAddress,
                Password = authenticationRequestBody.Password
            });

            var response = new AuthenticationResponseBody
            {
                IsAuthenticated = authenticationResponse.IsAuthenticated,
                JsonWebToken = authenticationResponse.JsonWebToken
            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("account")]
        public async Task<ServiceResponseGeneric<AuthenticationResponseBody>> Post([FromBody] AccountRequestBody authenticationRequestBody)
        {
            var accountResponse = await _accountService.CreateAccountAsync(new CreateAccountRequest
            {
                EmailAddress = authenticationRequestBody.EmailAddress,
                Password = authenticationRequestBody.Password
            });

            var response = new AuthenticationResponseBody
            {
                IsAuthenticated = true,
                JsonWebToken = accountResponse.JsonWebToken
            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.Created);
        }
    }
}
