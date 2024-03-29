using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services.Helper
{
    public interface IAuthenticationHelper
    {
        Task<PasswordHashResponse> GeneratePasswordHashAsync(PasswordHashRequest passwordHashRequest);
    }
}
