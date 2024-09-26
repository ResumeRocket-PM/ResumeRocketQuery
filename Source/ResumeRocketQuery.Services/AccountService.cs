using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        private readonly ISkillDataLayer _skillDataLayer;
        private readonly IEducationDataLayer _educationDataLayer;
        private readonly IExperienceDataLayer _experienceDataLayer;
        private readonly IAuthenticationHelper _authenticationHelper;

        public AccountService(IAuthenticationHelper authenticationHelper,
            IAccountDataLayer accountDataLayer, 
            IEmailAddressDataLayer emailAddressDataLayer,
            ILoginDataLayer loginDataLayer,
            IAuthenticationService authenticationService,
            ISkillDataLayer skillDataLayer,
            IEducationDataLayer educationDataLayer,
            IExperienceDataLayer experienceDataLayer
            )
        {
            _authenticationHelper = authenticationHelper;
            _accountDataLayer = accountDataLayer;
            _emailAddressDataLayer = emailAddressDataLayer;
            _loginDataLayer = loginDataLayer;
            _authenticationService = authenticationService;
            _skillDataLayer = skillDataLayer;
            _educationDataLayer = educationDataLayer;
            _experienceDataLayer = experienceDataLayer;
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
                FirstName = createAccountRequest.FirstName,
                LastName = createAccountRequest.LastName,
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

        public async Task<AccountDetails> GetAccountAsync(int accountId)
        {
            var account = _accountDataLayer.GetAccountAsync(accountId);

            var emailAddress = _emailAddressDataLayer.GetEmailAddressAsync(accountId);

            var skills = _skillDataLayer.GetSkillAsync(accountId);

            var education = _educationDataLayer.GetEducationAsync(accountId);

            var experience = _experienceDataLayer.GetExperienceAsync(accountId);

            await Task.WhenAll([account, emailAddress, skills, education, experience]);

            return new AccountDetails
            {
                AccountId = accountId,
                EmailAddress = emailAddress.Result.EmailAddress,
                PortfolioLink = account.Result.PortfolioLink,
                FirstName = account.Result.FirstName,
                LastName = account.Result.LastName,
                ProfilePhotoLink = account.Result.ProfilePhotoLink,
                StateLocation = account.Result.StateLocation,
                Title = account.Result.Title,
                Skills = skills.Result.Select(x => new Skill
                {
                    Description = x.Description,
                    SkillId = x.SkillId
                }).ToList(),
                Education = education.Result.Select(x => new Education
                {
                    AccountId = x.AccountId,
                    Degree = x.Degree,
                    EducationId = x.EducationId,
                    GraduationDate = x.GraduationDate,
                    Major = x.Major,
                    Minor = x.Minor,
                    SchoolName = x.SchoolName   
                }).ToList(),
                Experience = experience.Result.Select(x => new Experience
                {
                    AccountId = x.AccountId,
                    Company = x.Company,
                    Description = x.Description,
                    EndDate = x.EndDate,
                    ExperienceId = x.ExperienceId,
                    Position = x.Position,
                    StartDate = x.StartDate,
                    Type = x.Type   
                }).ToList(),
            };
        }

        public async Task UpdateAccount(int accountId, Dictionary<string, string> updates)
        {
            if(!updates.Any())
            {
                throw new ValidationException("Must pass Parameters to update");
            }

            var account = await _accountDataLayer.GetAccountAsync(accountId);

            var updatedAccount = new AccountStorage
            {
                AccountAlias = account.AccountAlias,
                AccountId = accountId,
                FirstName = updates.ContainsKey("FirstName") ? updates["FirstName"] : account.FirstName,
                LastName = updates.ContainsKey("LastName") ? updates["LastName"] : account.LastName,
                ProfilePhotoLink = updates.ContainsKey("ProfilePhotoLink") ? updates["ProfilePhotoLink"] : account.ProfilePhotoLink,
                Title = updates.ContainsKey("Title") ? updates["Title"] : account.Title,
                StateLocation = updates.ContainsKey("Location") ? updates["Location"] : account.StateLocation,
                PortfolioLink = updates.ContainsKey("PortfolioLink") ? updates["PortfolioLink"] : account.PortfolioLink,
            };

            await _accountDataLayer.UpdateAccountStorageAsync(updatedAccount);

            if(updates.ContainsKey("Education"))
            {
                var educations = JsonConvert.DeserializeObject<List<Education>>(updates["Education"]);

                await CreateEducationsAsync(accountId, educations);
            }

            if (updates.ContainsKey("Experience"))
            {
                var educations = JsonConvert.DeserializeObject<List<Experience>>(updates["Experience"]);

                await CreateExperiencesAsync(accountId, educations);
            }

            if (updates.ContainsKey("Skill"))
            {
                var educations = JsonConvert.DeserializeObject<List<Skill>>(updates["Skill"]);

                await CreateSkillsAsync(accountId, educations);
            }
        }

        public async Task CreateExperiencesAsync(int accountId, List<Experience> experiences)
        {
            await _experienceDataLayer.DeleteExperienceByAccountIdAsync(accountId);

            foreach (var experience in experiences)
            {
                await _experienceDataLayer.InsertExperienceAsync(new ExperienceStorage
                {
                    AccountId = experience.AccountId,
                    Company = experience.Company,
                    Description = experience.Description,
                    EndDate = experience.EndDate,
                    Position = experience.Position,
                    StartDate = experience.StartDate,
                });
            }
        }

        public async Task CreateEducationsAsync(int accountId, List<Education> educations)
        {
            await _educationDataLayer.DeleteEducationStorageAsync(accountId);

            foreach(var education in educations)
            {
                await _educationDataLayer.InsertEducationStorageAsync(new EducationStorage
                {
                    AccountId = education.AccountId,
                    Degree = education.Degree,
                    EducationId = education.EducationId,
                    GraduationDate = education.GraduationDate,
                    Major = education.Major,
                    Minor = education.Minor,
                    SchoolName = education.SchoolName
                });
            }
        }

        public async Task CreateSkillsAsync(int accountId, List<Skill> skills)
        {
            await _skillDataLayer.DeleteSkillByAccountIdAsync(accountId);

            foreach (var skill in skills)
            {
                await _skillDataLayer.InsertSkillAsync(new SkillStorage
                {
                    AccountId = accountId,
                    Description = skill.Description,
                });
            }
        }
    }
}
