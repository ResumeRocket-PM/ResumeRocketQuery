using System;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Helper;

namespace ResumeRocketQuery.Services
{
    public class AccountService : IAccountService
    {
        private readonly IResumeRocketQueryRepository _resumeRocketQueryRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthenticationHelper _authenticationHelper;

        public AccountService(IAuthenticationHelper authenticationHelper,
            IResumeRocketQueryRepository resumeRocketQueryRepository, 
            IAuthenticationService authenticationService)
        {
            _authenticationHelper = authenticationHelper;
            _resumeRocketQueryRepository = resumeRocketQueryRepository;
            _authenticationService = authenticationService;
        }

        public async Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest createAccountRequest)
        {
            var existingEmail = await _resumeRocketQueryRepository.GetAccountByEmailAddressAsync(createAccountRequest.EmailAddress);

            if (existingEmail != null)
            {
                throw new ValidationException("Account already exists");
            }

            var passwordSalt = Guid.NewGuid().ToString();

            var passwordResponse = await _authenticationHelper.GeneratePasswordHashAsync(new PasswordHashRequest
            {
                Salt = passwordSalt,
                Password = createAccountRequest.Password
            });

            var account = new Account
            {
                EmailAddress = createAccountRequest.EmailAddress,
                AccountAlias = Guid.NewGuid().ToString(),
                Authentication = new Authentication
                {
                    Salt = passwordSalt,
                    HashedPassword = passwordResponse.HashedPassword
                },
            };

            account.AccountId = await _resumeRocketQueryRepository.CreateAccountAsync(account);

            var jsonWebToken = _authenticationService.CreateJsonWebToken(account);

            return new CreateAccountResponse
            {
                AccountId = account.AccountId,
                JsonWebToken = jsonWebToken
            };
        }

        public async Task<Account> GetAccountAsync(int accountId)
        {
            var account = await _resumeRocketQueryRepository.GetAccountAsync(accountId);

            return account;
        }
    }
}
