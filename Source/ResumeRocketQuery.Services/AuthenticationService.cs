using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Helper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Domain.DataLayer;

namespace ResumeRocketQuery.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IEmailAddressDataLayer _emailAddressDataLayer;
        private readonly ILoginDataLayer _loginDataLayer;

        public AuthenticationService(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings,
            IAuthenticationHelper authenticationHelper,
            IEmailAddressDataLayer emailAddressDataLayer,
            ILoginDataLayer loginDataLayer)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
            _authenticationHelper = authenticationHelper;
            _emailAddressDataLayer = emailAddressDataLayer;
            _loginDataLayer = loginDataLayer;
        }

        public async Task<AuthenticateAccountResponse> AuthenticateAccountAsync(AuthenticateAccountRequest authenticateAccountRequest)
        {
            var result = new AuthenticateAccountResponse
            {
                IsAuthenticated = false
            };

            var account = await _emailAddressDataLayer.GetAccountByEmailAddressAsync(authenticateAccountRequest.EmailAddress);

            if (account != null)
            {
                var login = await _loginDataLayer.GetLoginAsync(account.AccountId);

                var passwordHashResponse = await _authenticationHelper.GeneratePasswordHashAsync(new PasswordHashRequest
                {
                    Salt = login.Salt,
                    Password = authenticateAccountRequest.Password
                });

                if (passwordHashResponse.HashedPassword == login.Hash)
                {
                    var jsonWebToken = CreateJsonWebToken(account.AccountId);

                    result.IsAuthenticated = true;
                    result.JsonWebToken = jsonWebToken;
                }
            }

            return result;
        }

        public string CreateJsonWebToken(int accountId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_resumeRocketQueryConfigurationSettings.AuthenticationPrivateKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("AccountId", accountId.ToString())
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
