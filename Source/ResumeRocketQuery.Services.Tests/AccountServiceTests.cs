using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Security.Principal;
using static ResumeRocketQuery.DataLayer.DataLayerConstants.StoredProcedures;
using System.Collections.Generic;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Services.Tests
{
    public class AccountServiceTests
    {
        private readonly IAccountService _systemUnderTest;
        private readonly IAccountDataLayer _accountDataLayer;
        private readonly IEmailAddressDataLayer _emailAddressDataLayer;
        private readonly ISkillDataLayer _skillDataLayer;
        private readonly IEducationDataLayer _educationDataLayer;
        private readonly IExperienceDataLayer _experienceDataLayer;

        public AccountServiceTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IAccountService>();

            _accountDataLayer = serviceProvider.GetService<IAccountDataLayer>();
            _emailAddressDataLayer = serviceProvider.GetService<IEmailAddressDataLayer>();
            _skillDataLayer = serviceProvider.GetService<ISkillDataLayer>();
            _educationDataLayer = serviceProvider.GetService<IEducationDataLayer>();
            _experienceDataLayer = serviceProvider.GetService<IExperienceDataLayer>();
        }

        public class CreateAccountAsync : AccountServiceTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_created()
            {
                var expected = new
                {
                    AccountId = Expect.NotDefault<int>()
                };

                var actual = await _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = $"{Guid.NewGuid().ToString()}@gmail.com",
                    Password = Guid.NewGuid().ToString(),
                    FirstName = "John",
                    LastName = "Doe"
                });

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_matches_response()
            {
                var emailAddress = $"{Guid.NewGuid().ToString()}@gmail.com";

                var createResponse = await _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = Guid.NewGuid().ToString(),
                });

                var expected = new
                {
                    EmailAddress = emailAddress,
                    AccountId = createResponse.AccountId,
                };

                var actual = await _systemUnderTest.GetAccountAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task GIVEN_account_exists_WHEN_CreateAccountAsync_is_called_THEN_validation_error_returned()
            {
                var emailAddress = $"{Guid.NewGuid().ToString()}@gmail.com";

                var createResponse = await _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = Guid.NewGuid().ToString()
                });

                var expected = new
                {
                    Message = "Account already exists"
                };

                var actual = await Assert.ThrowsAsync<ValidationException>(() => _systemUnderTest.CreateAccountAsync(new CreateAccountRequest
                {
                    EmailAddress = emailAddress,
                    Password = Guid.NewGuid().ToString()
                }));

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task GIVEN_account_created_WHEN_GetAccountAsync_is_called_THEN_account_matches_response()
            {
                var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
                {
                    FirstName = "John",
                    LastName = "Doe",
                    PortfolioLink = "https://portfolio.com/johndoe",
                    ProfilePhotoLink = "https://profilephoto.com/johndoe",
                    StateLocation = "CA",
                    Title = "Software Engineer",
                    AccountAlias = Guid.NewGuid().ToString()
                });

                var emailAddress = $"{Guid.NewGuid().ToString()}@gmail.com";

                await _emailAddressDataLayer.InsertEmailAddressStorageAsync(new EmailAddressStorage
                {
                    AccountId = accountId,
                    EmailAddress = emailAddress
                });

                await _skillDataLayer.InsertSkillAsync(new SkillStorage { AccountId = accountId, Description = "C#" });
                await _skillDataLayer.InsertSkillAsync(new SkillStorage { AccountId = accountId, Description = "ASP.NET" });

                await _educationDataLayer.InsertEducationStorageAsync(new EducationStorage
                {
                    AccountId = accountId,
                    Degree = "Bachelor's",
                    SchoolName = "MIT",
                    Major = "Computer Science",
                    GraduationDate = new DateTime(2020, 5, 1)
                });

                await _experienceDataLayer.InsertExperienceAsync(new ExperienceStorage
                {
                    AccountId = accountId,
                    Company = "Tech Corp",
                    Position = "Software Engineer",
                    StartDate = new DateTime(2020, 6, 1),
                    EndDate = new DateTime(2022, 7, 1),
                    Description = "Developed web applications.",
                    Type = "FullTime"
                });

                var expected = new
                {
                    AccountId = accountId,
                    EmailAddress = emailAddress,
                    PortfolioLink = "https://portfolio.com/johndoe",
                    FirstName = "John",
                    LastName = "Doe",
                    ProfilePhotoLink = "https://profilephoto.com/johndoe",
                    StateLocation = "CA",
                    Title = "Software Engineer",
                    Skills = new[]
                    {
                        new
                        { 
                            Description = "C#"
                        },
                        new
                        {
                            Description = "ASP.NET"
                        },
                    },
                    Education = new[]
                    {
                        new
                        {
                            AccountId = accountId,
                            Degree = "Bachelor's",
                            EducationId = Expect.Any<int>(x => x > 0),
                            GraduationDate = new DateTime(2020, 5, 1),
                            Major = "Computer Science",
                            Minor = (string)null,
                            SchoolName = "MIT"
                        }
                    },
                    Experience = new[]
                    {
                        new
                        {
                            AccountId = accountId,
                            Company = "Tech Corp",
                            Description = "Developed web applications.",
                            EndDate = new DateTime(2022, 7, 1),
                            ExperienceId = Expect.Any<int>(x => x > 0),
                            Position = "Software Engineer",
                            StartDate = new DateTime(2020, 6, 1),
                            Type = "FullTime"
                        }
                    }
                };

                var actual = await _systemUnderTest.GetAccountAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
