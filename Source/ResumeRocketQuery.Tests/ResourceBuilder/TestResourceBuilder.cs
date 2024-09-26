using ResumeRocketQuery.Domain.DataLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Tests.ResourceBuilder
{
    public class TestResourceBuilder
    {
        private readonly IAccountService _accountService;
        private readonly ISkillDataLayer _skillDataLayer;
        private readonly IEducationDataLayer _educationDataLayer;
        private readonly IExperienceDataLayer _experienceDataLayer;

        private Faker _faker = new Faker();

        string _FirstName = null;
        string _LastName = null;
        string _PortfolioLink = null;
        string _ProfilePhotoLink = null;
        string _StateLocation = null;
        string _Title = null;
        string _AccountAlias = null;


        public TestResourceBuilder(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public TestResourceBuilder Initialize()
        {
            _FirstName = _faker.Name.FirstName();
            _LastName = _faker.Name.LastName();
            _PortfolioLink = _faker.Internet.Url();
            _ProfilePhotoLink = _faker.Internet.Avatar();
            _StateLocation = _faker.Address.StateAbbr();
            _Title = _faker.Name.JobTitle();
            _AccountAlias = Guid.NewGuid().ToString();
            _emailAddress = _faker.Internet.Email();
            _password = "password";

            return this;
        }

        string _password = null;
        public TestResourceBuilder SetPassword(string password)
        {
            _password = password;

            return this;
        }

        string _emailAddress = null;
        public TestResourceBuilder SetEmailAddress(string emailAddress)
        {
            _emailAddress = emailAddress;

            return this;
        }

        List<Domain.Services.Repository.Education> _educations = new List<Domain.Services.Repository.Education>();
        public TestResourceBuilder AddEducation(int numberOfEducations)
        {
            DateTime lastGraduationDate = _faker.Date.Past(10);

            for (int i = 0; i < numberOfEducations; i++)
            {
                var startDate = lastGraduationDate.AddMonths(_faker.Random.Int(1, 6));
                var graduationDate = startDate.AddYears(_faker.Random.Int(2, 5));

                _educations.Add(new Domain.Services.Repository.Education
                {
                    Degree = _faker.PickRandom("Bachelor's", "Master's", "PhD"),
                    SchoolName = _faker.PickRandom(new List<string> { "MIT", "Harvard", "Stanford", "UC Berkeley", "Oxford" }),
                    Major = _faker.PickRandom(new List<string> { "Computer Science", "Mechanical Engineering", "Business Administration", "Psychology", "Biology" }),
                    GraduationDate = graduationDate
                });

                lastGraduationDate = graduationDate;
            }

            return this;
        }

        List<Domain.Services.Repository.Experience> _experiences = new List<Domain.Services.Repository.Experience>();
        public TestResourceBuilder AddExperience(int numberOfExperiences)
        {
            DateTime lastEndDate = _faker.Date.Past(10);

            for (int i = 0; i < numberOfExperiences; i++)
            {
                var startDate = lastEndDate.AddMonths(_faker.Random.Int(1, 6));
                var endDate = startDate.AddYears(_faker.Random.Int(1, 3));

                _experiences.Add(new Domain.Services.Repository.Experience
                {
                    Company = _faker.Company.CompanyName(),
                    Description = _faker.Lorem.Sentence(),
                    Position = _faker.Name.JobTitle(),
                    Type = _faker.PickRandom("FullTime", "PartTime", "Contract"),
                    StartDate = startDate,
                    EndDate = endDate
                });

                lastEndDate = endDate;
            }

            return this;
        }

        List<Skill> _skills = new List<Skill>();
        public TestResourceBuilder AddSkills(int number = 5)
        {
            for (int i = 0; i < number; i++)
            {
                _skills.Add(new Skill
                {
                    Description = _faker.Hacker.Verb() + " " + _faker.Hacker.Noun()
                });
            }

            return this;
        }


        public async Task<int> Create()
        {
            var result = await _accountService.CreateAccountAsync(new CreateAccountRequest
            {
                EmailAddress = _emailAddress,
                FirstName = _FirstName,
                LastName = _LastName,
                Password = _password
            });

            var parameters = new Dictionary<string, string> {
                {"PortfolioLink", _PortfolioLink},
                {"ProfilePhotoLink", _ProfilePhotoLink},
                {"Location", _StateLocation},
                {"Title", _Title},
            };

            await _accountService.UpdateAccount(result.AccountId, parameters);

            await _accountService.CreateSkillsAsync(result.AccountId, _skills);

            await _accountService.CreateExperiencesAsync(result.AccountId, _experiences);

            await _accountService.CreateEducationsAsync(result.AccountId, _educations);

            return result.AccountId;
        }


    }
}
