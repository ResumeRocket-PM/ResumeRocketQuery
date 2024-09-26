using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
        public async Task<ServiceResponseGeneric<AccountResponseBody>> Get()
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var accountResponse = await _accountService.GetAccountAsync(user.AccountId);

            var response = new AccountResponseBody
            {
                FirstName = accountResponse.FirstName,
                LastName = accountResponse.LastName,
                ProfilePhotoLink = accountResponse.ProfilePhotoLink,
                Title = accountResponse.Title,
                Email = accountResponse.EmailAddress,
                Location = accountResponse.StateLocation,
                PortfolioLink = accountResponse.PortfolioLink,
                Resume = null,
                Skills = accountResponse.Skills,
                Experience = accountResponse.Experience.Select(e => new Domain.Api.Response.Experience
                {
                    Company = e.Company,
                    Description = e.Description,
                    EndDate = e.EndDate,
                    Position = e.Position,
                    StartDate = e.StartDate,
                    Type = e.Type
                }).ToList(),
                Education = accountResponse.Education.Select(edu => new Domain.Api.Response.Education
                {
                    Degree = edu.Degree,
                    GraduationDate = edu.GraduationDate,
                    Major = edu.Major,
                    Minor = edu.Minor,
                    SchoolName = edu.SchoolName
                }).ToList()
            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("details")]
        public async Task<ServiceResponse> ModifyAccount([FromBody] AccountDetailsRequest accountDetailsRequest)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            await _accountService.UpdateAccount(user.AccountId, accountDetailsRequest.Parameters );

            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }
    }
}
