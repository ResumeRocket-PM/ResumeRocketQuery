using System;
using System.Linq;
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
            var account = await _accountDataLayer.GetAccountAsync(accountId);

            var emailAddress = await _emailAddressDataLayer.GetEmailAddressAsync(accountId);

            var skills = await _skillDataLayer.GetSkillAsync(accountId);

            var education = await _educationDataLayer.GetEducationAsync(accountId);

            var experience = await _experienceDataLayer.GetExperienceAsync(accountId);

            return new AccountDetails
            {
                AccountId = accountId,
                EmailAddress = emailAddress.EmailAddress,
                PortfolioLink = account.PortfolioLink,
                FirstName = account.FirstName,
                LastName = account.LastName,
                ProfilePhotoLink = account.ProfilePhotoLink,
                StateLocation = account.StateLocation,
                Title = account.Title,
                Skills = skills.Select(x => x.Description).ToList(),
                Education = education.Select(x => new Education
                {
                    AccountId = x.AccountId,
                    Degree = x.Degree,
                    EducationId = x.EducationId,
                    GraduationDate = x.GraduationDate,
                    Major = x.Major,
                    Minor = x.Minor,
                    SchoolName = x.SchoolName   
                }).ToList(),
                Experience = experience.Select(x => new Experience
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
    }
}
