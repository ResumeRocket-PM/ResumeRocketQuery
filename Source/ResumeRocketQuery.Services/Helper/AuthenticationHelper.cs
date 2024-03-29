using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Helper;

namespace ResumeRocketQuery.Services.Helper
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        public Task<PasswordHashResponse> GeneratePasswordHashAsync(PasswordHashRequest passwordHashRequest)
        {
            using (var sha512 = SHA512.Create())
            {
                var formattedPassword = $"{passwordHashRequest.Salt}{passwordHashRequest.Password}";

                var byteArray = Encoding.Default.GetBytes(formattedPassword);

                var hashByteArray = sha512.ComputeHash(byteArray);

                var hashedPassword = Convert.ToBase64String(hashByteArray);

                var result = new PasswordHashResponse
                {
                    HashedPassword = hashedPassword,
                };

                return Task.FromResult(result);
            }
        }
    }
}
