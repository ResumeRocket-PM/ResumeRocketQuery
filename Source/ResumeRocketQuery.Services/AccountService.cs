using System;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Helper;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountDataLayer _accountDataLayer;
        private readonly IEmailAddressDataLayer _emailAddressDataLayer;
        private readonly ILoginDataLayer _loginDataLayer;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthenticationHelper _authenticationHelper;

        public AccountService(IAuthenticationHelper authenticationHelper,
            IAccountDataLayer accountDataLayer, 
            IEmailAddressDataLayer emailAddressDataLayer,
            ILoginDataLayer loginDataLayer,
            IAuthenticationService authenticationService)
        {
            _authenticationHelper = authenticationHelper;
            _accountDataLayer = accountDataLayer;
            _emailAddressDataLayer = emailAddressDataLayer;
            _loginDataLayer = loginDataLayer;
            _authenticationService = authenticationService;
        }

        public async Task<CreateAccountResponse> CreateAccountAsync(CreateAccountRequest createAccountRequest)
        {
            var existingEmail = await _emailAddressDataLayer.GetAccountByEmailAddressAsync(createAccountRequest.EmailAddress);

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

            var accountStorage = new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
            };

            accountStorage.AccountId = await _accountDataLayer.InsertAccountStorageAsync(accountStorage);

            await _emailAddressDataLayer.InsertEmailAddressStorageAsync(new EmailAddressStorage
            {
                AccountId = accountStorage.AccountId,
                EmailAddress = createAccountRequest.EmailAddress
            });

            await _loginDataLayer.InsertLoginStorageAsync(new LoginStorage
            {
                AccountId = accountStorage.AccountId,
                Hash = passwordResponse.HashedPassword,
                Salt = passwordSalt
            });

            var jsonWebToken = _authenticationService.CreateJsonWebToken(accountStorage.AccountId);

            return new CreateAccountResponse
            {
                AccountId = accountStorage.AccountId,
                JsonWebToken = jsonWebToken
            };
        }

        public async Task<Account> GetAccountAsync(int accountId)
        {
            var account = await _accountDataLayer.GetAccountAsync(accountId);

            return account;
        }
    }
}
