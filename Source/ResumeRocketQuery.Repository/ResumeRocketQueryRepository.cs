using System;
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


        public async Task<Portfolio> GetPortfolioAsync(int accountId)
        {
            Portfolio result = null;

            var portfolioStorage = await _resumeRocketQueryStorage.SelectPortfolioStorageAsync(accountId);

            if (portfolioStorage != null)
            {
                result = new Portfolio
                {
                    AccountId = portfolioStorage.AccountId,
                    Configuration = portfolioStorage.PortfolioConfiguration
                };
            }

            return result;
        }

        public async Task<int> CreatePortfolioAsync(Portfolio portfolio)
        {
            Portfolio result = null;

            var portfolioId = await _resumeRocketQueryStorage.InsertPortfolioStorageAsync(new PortfolioStorage
            {
                AccountId = portfolio.AccountId,
                PortfolioAlias = Guid.NewGuid().ToString(),
                PortfolioConfiguration = portfolio.Configuration
            });

            return portfolioId;
        }


        public async Task<Resume> GetResumeAsync(int accountId)
        {
            Resume result = null;

            var resumeStorage = await _resumeRocketQueryStorage.SelectResumeStorageAsync(accountId);

            if (resumeStorage != null)
            {
                result = new Resume
                {
                    AccountId = resumeStorage.AccountId,
                    Content = resumeStorage.ResumeContent
                };
            }

            return result;
        }

        public async Task<int> CreateResumeAsync(Resume resume)
        {
            Resume result = null;

            var resumeId = await _resumeRocketQueryStorage.InsertResumeStorageAsync(new ResumeStorage
            {
                AccountId = resume.AccountId,
                ResumeAlias = Guid.NewGuid().ToString(),
                ResumeContent = resume.Content
            });

            return resumeId;
        }
    }
}
