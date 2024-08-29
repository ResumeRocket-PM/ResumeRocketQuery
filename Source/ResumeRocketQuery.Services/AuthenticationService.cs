using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Helper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace ResumeRocketQuery.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;
        private readonly IResumeRocketQueryRepository _resumeRocketQueryRepository;
        private readonly IAuthenticationHelper _authenticationHelper;

        public AuthenticationService(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings,
            IResumeRocketQueryRepository resumeRocketQueryRepository,
            IAuthenticationHelper authenticationHelper)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
            _resumeRocketQueryRepository = resumeRocketQueryRepository;
            _authenticationHelper = authenticationHelper;
        }

        public async Task<AuthenticateAccountResponse> AuthenticateAccountAsync(AuthenticateAccountRequest authenticateAccountRequest)
        {
            var result = new AuthenticateAccountResponse
            {
                IsAuthenticated = false
            };

            var account = await _resumeRocketQueryRepository.GetAccountByEmailAddressAsync(authenticateAccountRequest.EmailAddress);

            if (account != null)
            {
                var passwordHashResponse = await _authenticationHelper.GeneratePasswordHashAsync(new PasswordHashRequest
                {
                    Salt = account.Authentication.Salt,
                    Password = authenticateAccountRequest.Password
                });

                if (passwordHashResponse.HashedPassword == account.Authentication.HashedPassword)
                {
                    var jsonWebToken = CreateJsonWebToken(account);

                    result.IsAuthenticated = true;
                    result.JsonWebToken = jsonWebToken;
                }
            }

            return result;
        }

        public string CreateJsonWebToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_resumeRocketQueryConfigurationSettings.AuthenticationPrivateKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("AccountId", account.AccountId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var result = tokenHandler.WriteToken(token);

            return result;
        }

        public bool ValidateJsonWebToken(string jsonWebToken)
        {
            var result = false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_resumeRocketQueryConfigurationSettings.AuthenticationPrivateKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true
            };

            try
            {
                var token = tokenHandler.ValidateToken(jsonWebToken, tokenValidationParameters, out var validatedToken);
                result = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e); // ignored
            }

            return result;
        }
    }
}
