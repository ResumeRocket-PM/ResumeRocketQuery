using System.Threading.Tasks;
using Newtonsoft.Json;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Repository
{
    public class ResumeRocketQueryRepository : IResumeRocketQueryRepository
    {
        private readonly IResumeRocketQueryStorage _resumeRocketQueryStorage;

        public ResumeRocketQueryRepository(IResumeRocketQueryStorage resumeRocketQueryStorage)
        {
            _resumeRocketQueryStorage = resumeRocketQueryStorage;
        }

        public async Task<int> CreateAccountAsync(Account account)
        {
            var accountConfiguration = new AccountConfigurationStorage
            {
                AccountAlias = account.AccountAlias,
                Authentication = new Authentication
                {
                    HashedPassword = account.Authentication.HashedPassword,
                    Salt = account.Authentication.Salt
                }
            };

            var result = await _resumeRocketQueryStorage.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = account.AccountAlias,
                AccountConfiguration = JsonConvert.SerializeObject(accountConfiguration),
            });

            await _resumeRocketQueryStorage.InsertEmailAddressStorageAsync(new EmailAddressStorage
            {
                EmailAddress = account.EmailAddress,
                AccountId = result,
            });

            return result;
        }

        public async Task<Account> GetAccountAsync(int accountId)
        {
            Account result = null;

            var accountStorage = await _resumeRocketQueryStorage.SelectAccountStorageAsync(accountId);

            if (accountStorage != null)
            {
                var emailAddressStorage = await _resumeRocketQueryStorage.SelectEmailAddressStorageByAccountIdAsync(accountStorage.AccountId);

                var accountConfiguration = JsonConvert.DeserializeObject<AccountConfigurationStorage>(accountStorage.AccountConfiguration);

                result = new Account
                {
                    AccountId = accountStorage.AccountId,
                    AccountAlias = accountStorage.AccountAlias,
                    EmailAddress = emailAddressStorage.EmailAddress,
                    Authentication = new Authentication
                    {
                        Salt = accountConfiguration.Authentication.Salt,
                        HashedPassword = accountConfiguration.Authentication.HashedPassword
                    }
                };
            }

            return result;
        }

        public async Task<Account> GetAccountByEmailAddressAsync(string emailAddress)
        {
            Account result = null;

            var emailAddressStorage = await _resumeRocketQueryStorage.SelectEmailAddressStorageByEmailAddressAsync(emailAddress);

            if (emailAddressStorage != null)
            {
                var accountStorage = await _resumeRocketQueryStorage.SelectAccountStorageAsync(emailAddressStorage.AccountId);

                var accountConfiguration = JsonConvert.DeserializeObject<AccountConfigurationStorage>(accountStorage.AccountConfiguration);

                result = new Account
                {
                    AccountId = accountStorage.AccountId,
                    AccountAlias = accountStorage.AccountAlias,
                    EmailAddress = emailAddressStorage.EmailAddress,
                    Authentication = new Authentication
                    {
                        Salt = accountConfiguration.Authentication.Salt,
                        HashedPassword = accountConfiguration.Authentication.HashedPassword
                    }
                };
            }

            return result;
        }
    }
}
