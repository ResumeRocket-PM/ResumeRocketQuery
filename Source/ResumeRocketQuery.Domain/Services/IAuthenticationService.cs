using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticateAccountResponse> AuthenticateAccountAsync(AuthenticateAccountRequest authenticateAccountRequest);
        string CreateJsonWebToken(Account account);
        bool ValidateJsonWebToken(string jsonWebToken);
    }
}
