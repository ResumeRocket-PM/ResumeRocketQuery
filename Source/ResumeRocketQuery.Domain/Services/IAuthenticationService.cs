using ResumeRocketQuery.Domain.Services.Repository;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticateAccountResponse> AuthenticateAccountAsync(AuthenticateAccountRequest authenticateAccountRequest);
        string CreateJsonWebToken(int accountId);
        bool ValidateJsonWebToken(string jsonWebToken);
    }
}
