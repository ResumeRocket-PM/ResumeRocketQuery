using System.Security.Claims;

namespace ResumeRocketQuery.Domain.Api
{
    public interface IResumeRocketQueryUserBuilder
    {
        ResumeRocketQueryUser GetResumeRocketQueryUser(ClaimsPrincipal claimsPrincipal);
    }
}
