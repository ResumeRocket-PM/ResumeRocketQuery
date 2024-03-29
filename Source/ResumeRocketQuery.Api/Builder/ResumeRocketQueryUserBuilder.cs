using System.Linq;
using System.Security.Claims;
using ResumeRocketQuery.Domain.Api;

namespace ResumeRocketQuery.Api.Builder
{
    public class ResumeRocketQueryUserBuilder : IResumeRocketQueryUserBuilder
    {
        public ResumeRocketQueryUser GetResumeRocketQueryUser(ClaimsPrincipal claimsPrincipal)
        {
            var accountIdClaim = claimsPrincipal.Claims.First(x => x.Type == "AccountId");

            var result = new ResumeRocketQueryUser
            {
                AccountId = int.Parse(accountIdClaim.Value)
            };

            return result;
        }
    }
}
