using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;

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
                },
                Title = account.Title,
                Skills = account.Skills,    
                ProfilePhotoUrl = account.ProfilePhotoUrl,
                PortfolioLink = account.PortfolioLink,
                Experience = account.Experience,
                Education = account.Education,
                Location = account.Location,
                Name = account.Name
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
                    },
                    
                    Name = accountConfiguration.Name,
                    Location = accountConfiguration.Location,
                    Education = accountConfiguration.Education,
                    Experience = accountConfiguration.Experience,
                    PortfolioLink = accountConfiguration.PortfolioLink,
                    ProfilePhotoUrl = accountConfiguration.ProfilePhotoUrl,
                    Skills = accountConfiguration.Skills,
                    Title = accountConfiguration.Title,
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
            Portfolio result = null; //TODO is this needed?

            var portfolioId = await _resumeRocketQueryStorage.InsertPortfolioStorageAsync(new PortfolioStorage
            {
                AccountId = portfolio.AccountId,
                PortfolioAlias = Guid.NewGuid().ToString(),
                PortfolioConfiguration = portfolio.Configuration
            });

            return portfolioId;
        }

        public async Task<List<Resume>> GetResumesAsync(int accountId)
        {
            var resumeStorage = await _resumeRocketQueryStorage.SelectResumeStoragesAsync(accountId);

            var result = resumeStorage.Select(x => new Resume
            {
                AccountID = x.accountID,
                ApplyDate = x.applyDate,
                CompanyName = x.companyName,
                JobUrl = x.jobUrl,
                Position = x.position,
                ResumeContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(x.resume),
                ResumeID = x.ResumeID,
                Status = x.status,
            });

            return result.ToList();
        }

        public async Task<int> CreateResumeAsync(Resume resume)
        {
            var resumeId = await _resumeRocketQueryStorage.InsertResumeStorageAsync(new ResumeStorage
            {
                ResumeID = resume.ResumeID,
                accountID = resume.AccountID,
                applyDate = resume.ApplyDate,
                companyName = resume.CompanyName,
                resume = JsonConvert.SerializeObject(resume.ResumeContent),
                position = resume.Position,
                jobUrl = resume.JobUrl,
                status = resume.Status,
            });

            return resumeId;
        }


        public async Task<Resume> GetResumeAsync(int resumeId)
        {
            Resume result = null;

            var resumeStorage = await _resumeRocketQueryStorage.SelectResumeStorageAsync(resumeId);

            if (resumeStorage != null)
            {
                result = new Resume
                {
                    AccountID = resumeStorage.accountID,
                    ApplyDate = resumeStorage.applyDate,
                    CompanyName = resumeStorage.companyName,
                    JobUrl = resumeStorage.jobUrl,
                    Position = resumeStorage.position,
                    ResumeContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(resumeStorage.resume),
                    ResumeID = resumeStorage.ResumeID,
                    Status = resumeStorage.status,
                };
            }

            return result;
        }

        public async Task UpdateResume(ResumeStorage resumeStorage)
        {
            try
            {
                await _resumeRocketQueryStorage.UpdateResumeStorageAsync(resumeStorage);
            }
            catch (Exception)
            {

                throw;
            }
            //if ()

            //throw new NotImplementedException();
        }
    }
}
