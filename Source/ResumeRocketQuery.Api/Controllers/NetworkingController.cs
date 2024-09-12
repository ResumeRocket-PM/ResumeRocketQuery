using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iText.Layout.Element;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;

namespace ResumeRocketQuery.Api.Controllers
{
    /// <summary>
    /// This is a controller that handles the creation and modification of accounts.
    /// </summary>
    [Authorize]
    [Route("api/networking")]
    public class NetworkingController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly INetworkingService _networkingService;

        public NetworkingController(IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder,
            INetworkingService networkingService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _networkingService = networkingService;
        }

        /// <summary>
        /// This retrieves the User details
        /// </summary>
        /// <returns>A User Object.</returns>
        [HttpGet]
        [Route("{filter}/{searchterm}")]
        public async Task<ServiceResponseGeneric<List<AccountResponseBody>>> Get([FromRoute] string filter, string searchterm)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            if (new[]{"Name"}.Contains(filter))
            {
                throw new ValidationException("filter is invalid");
            }

            var accountResponse = await _networkingService.FilterAccounts(filter, searchterm);

            var response = accountResponse.Select(x => new AccountResponseBody
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                ProfilePhotoLink = x.ProfilePhotoLink,
                Title = x.Title,
                Email = x.EmailAddress,
                Location = x.StateLocation,
                PortfolioLink = x.PortfolioLink,
                Resume = null,
                Skills = x.Skills,
                Experience = x.Experience.Select(e => new Experience
                {
                    Company = e.Company,
                    Description = e.Description,
                    EndDate = e.EndDate,
                    Position = e.Position,
                    StartDate = e.StartDate,
                    Type = e.Type
                }).ToList(),
                Education = x.Education.Select(edu => new Education
                {
                    Degree = edu.Degree,
                    GraduationDate = edu.GraduationDate,
                    Major = edu.Major,
                    Minor = edu.Minor,
                    SchoolName = edu.SchoolName
                }).ToList()
            }).ToList();

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.OK);
        }
    }
}
