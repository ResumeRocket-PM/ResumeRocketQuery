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
    [Route("api/portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly IPortfolioService _portfolioService;

        public PortfolioController(IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder,
            IPortfolioService portfolioService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _portfolioService = portfolioService;
        }

        /// <summary>
        /// This retrieves the Portfolio details
        /// </summary>
        /// <returns>A User Object.</returns>
        [HttpGet]
        [Route("details")]
        public async Task<ServiceResponseGeneric<PortfolioResponseBody>> Get()
        {
            var account = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var portfolio = await _portfolioService.GetPortfolio(account.AccountId);

            if (portfolio != null)
            {
                var response = new PortfolioResponseBody
                {
                    Content = portfolio?.Configuration ?? string.Empty
                };

                return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.OK);
            }

            return _serviceResponseBuilder.BuildServiceResponse<PortfolioResponseBody>(null, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// This retrieves the Portfolio details by a specific PortfolioId
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns>A User Object.</returns>
        [HttpGet]
        [Route("{portfolioId}/details")]
        public async Task<ServiceResponseGeneric<PortfolioResponseBody>> Get([FromRoute] int portfolioId)
        {
            var portfolio = await _portfolioService.GetPortfolioByPortfolioId(portfolioId);

            if (portfolio != null)
            {
                var response = new PortfolioResponseBody
                {
                    Content = portfolio?.Configuration ?? string.Empty
                };

                return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.OK);
            }

            return _serviceResponseBuilder.BuildServiceResponse<PortfolioResponseBody>(null, HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("")]
        public async Task<ServiceResponse> Post([FromBody] PortfolioRequestBody requestBody)
        {
            var account = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            int portfolioId = await _portfolioService.CreatePortfolio(new Portfolio
            {
                Configuration = requestBody.Content,
                AccountId = account.AccountId,
            });

            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.Created);
        }
    }
}
