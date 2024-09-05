using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ResumeRocketQuery.Domain.DataLayer;
using System.Data;

namespace ResumeRocketQuery.Repository.Tests
{
    public class ResumeRocketQueryRepositoryTests
    {
        private IResumeRocketQueryRepository _systemUnderTest;

        public ResumeRocketQueryRepositoryTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IResumeRocketQueryRepository>();
        }

        public class CreateAccountAsync : ResumeRocketQueryRepositoryTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_stored()
            {
                var accountId = await _systemUnderTest.CreateAccountAsync(new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    },


                    Name = "John Doe",
                    ProfilePhotoUrl = "https://link/to/profile/photo",
                    Title = "Software Engineer",
                    Location = "Salt Lake City, UT",
                    PortfolioLink = "https://resume-rocket/u1234567/portfolio",
                    Resume = "<<resume>>",
                    Skills = new List<string> { "Java", "Python", "C++" },
                    Experience = new List<Experience>
                    {
                        new Experience
                        {
                            Company = "Google",
                            Position = "Software Engineer",
                            Type = "<<Full-time/part-time/Internship>>",
                            Description = "worked on google search team, improved algorithm",
                            StartDate = DateTime.Parse("01/01/2019"),
                            EndDate = DateTime.Parse("01/01/2021")
                        },
                    },
                    Education = new List<Education>
                    {
                        new Education
                        {
                            SchoolName = "University of Utah",
                            Degree = "BS",
                            Major = "Computer Science",
                            Minor = "Music",
                            GraduationDate = DateTime.Parse("01/01/2019"),
                            Courses = new List<string> { "Data Structures", "Algorithms", "Computer Networks" }
                        },
                    }
                });

                Assert.True(accountId > 0);
            }

            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_storage_matches()
            {
                var expected = new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    },

                    Name = "John Doe",
                    ProfilePhotoUrl = "https://link/to/profile/photo",
                    Title = "Software Engineer",
                    Location = "Salt Lake City, UT",
                    PortfolioLink = "https://resume-rocket/u1234567/portfolio",
                    Skills = new List<string> { "Java", "Python", "C++" },
                    Experience = new List<Experience>
                    {
                        new Experience
                        {
                            Company = "Google",
                            Position = "Software Engineer",
                            Type = "<<Full-time/part-time/Internship>>",
                            Description = "worked on google search team, improved algorithm",
                            StartDate = DateTime.Parse("01/01/2019"),
                            EndDate = DateTime.Parse("01/01/2021")
                        },
                    },
                    Education = new List<Education>
                    {
                        new Education
                        {
                            SchoolName = "University of Utah",
                            Degree = "BS",
                            Major = "Computer Science",
                            Minor = "Music",
                            GraduationDate = DateTime.Parse("01/01/2019"),
                            Courses = new List<string> { "Data Structures", "Algorithms", "Computer Networks" }
                        },
                    }
                };

                expected.AccountId = await _systemUnderTest.CreateAccountAsync(expected);

                var actual = await _systemUnderTest.GetAccountAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class GetAccountByEmailAddressAsync : ResumeRocketQueryRepositoryTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_storage_matches()
            {
                var expected = new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    },

                    Name = "John Doe",
                    ProfilePhotoUrl = "https://link/to/profile/photo",
                    Title = "Software Engineer",
                    Location = "Salt Lake City, UT",
                    PortfolioLink = "https://resume-rocket/u1234567/portfolio",
                    Resume = "<<resume>>",
                    Skills = new List<string> { "Java", "Python", "C++" },
                    Experience = new List<Experience>
                    {
                        new Experience
                        {
                            Company = "Google",
                            Position = "Software Engineer",
                            Type = "<<Full-time/part-time/Internship>>",
                            Description = "worked on google search team, improved algorithm",
                            StartDate = DateTime.Parse("01/01/2019"),
                            EndDate = DateTime.Parse("01/01/2021")
                        },
                    },
                    Education = new List<Education>
                    {
                        new Education
                        {
                            SchoolName = "University of Utah",
                            Degree = "BS",
                            Major = "Computer Science",
                            Minor = "Music",
                            GraduationDate = DateTime.Parse("01/01/2019"),
                            Courses = new List<string> { "Data Structures", "Algorithms", "Computer Networks" }
                        },
                    }
                };

                expected.AccountId = await _systemUnderTest.CreateAccountAsync(expected);

                var actual = await _systemUnderTest.GetAccountByEmailAddressAsync(expected.EmailAddress);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class CreatePortfolioAsync : ResumeRocketQueryRepositoryTests
        {
            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_account_is_stored()
            {
                var accountId = await _systemUnderTest.CreateAccountAsync(new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    },
                });

                var portfolioId = await _systemUnderTest.CreatePortfolioAsync(new Portfolio
                {
                    Configuration = Guid.NewGuid().ToString(),
                    AccountId = accountId,
                });

                Assert.True(portfolioId > 0);
            }

            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_storage_matches()
            {

                var accountId = await _systemUnderTest.CreateAccountAsync(new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    }
                });

                var portfolio = Guid.NewGuid().ToString();

                var portfolioId = await _systemUnderTest.CreatePortfolioAsync(new Portfolio
                {
                    Configuration = portfolio,
                    AccountId = accountId,
                });

                var expected = new Portfolio
                {
                    AccountId = accountId,
                    Configuration = portfolio
                };

                var actual = await _systemUnderTest.GetPortfolioAsync(expected.AccountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class CreateResumeAsync : ResumeRocketQueryRepositoryTests
        {
            [Fact]
            public async Task WHEN_CreateResumeAsyncc_is_called_THEN_account_is_stored()
            {
                var accountId = await _systemUnderTest.CreateAccountAsync(new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    }
                });

                var portfolioId = await _systemUnderTest.CreateResumeAsync(new Resume
                {
                    AccountID = accountId,
                    ApplyDate = DateTime.Today,
                    CompanyName = "Amazon",
                    JobUrl = "amazon.com/job",
                    Position = "Software Engineer",
                    ResumeContent = new Dictionary<string, string>
                    {
                        {"FileType", ".Pdf"},
                        {"Bytes",Guid.NewGuid().ToString()}
                    },
                    Status = "Pending"
                });

                Assert.True(portfolioId > 0);
            }

            [Fact]
            public async Task WHEN_CreateAccountAsync_is_called_THEN_storage_matches()
            {
                var accountId = await _systemUnderTest.CreateAccountAsync(new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    }
                });

                var expected = new Resume
                {
                    AccountID = accountId,
                    ApplyDate = DateTime.Today,
                    CompanyName = "Amazon",
                    JobUrl = "amazon.com/job",
                    Position = "Software Engineer",
                    ResumeContent = new Dictionary<string, string>
                    {
                        {"FileType", ".Pdf"},
                        {"Bytes",Guid.NewGuid().ToString()}
                    },
                    Status = "Pending"
                };

                expected.ResumeID = await _systemUnderTest.CreateResumeAsync(expected);

                var actual = await _systemUnderTest.GetResumeAsync(expected.ResumeID);

                expected.ToExpectedObject().ShouldMatch(actual);
            }
        }

        public class GetResumesAsync : ResumeRocketQueryRepositoryTests
        {
            [Fact]
            public async Task WHEN_GetResumesAsync_is_called_THEN_storage_matches()
            {
                var accountId = await _systemUnderTest.CreateAccountAsync(new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    }
                });

                var expected = new[]
                {
                    new Resume
                    {
                        AccountID = accountId,
                        ApplyDate = DateTime.Today,
                        CompanyName = "Amazon",
                        JobUrl = "amazon.com/job",
                        Position = "Software Engineer",
                        ResumeContent = new Dictionary<string, string>
                        {
                            {"FileType", ".Pdf"},
                            {"Bytes",Guid.NewGuid().ToString()}
                        },
                        Status = "Pending"
                    },
                    new Resume
                    {
                        AccountID = accountId,
                        ApplyDate = DateTime.Today,
                        CompanyName = "Facebook",
                        JobUrl = "facebook.com/job",
                        Position = "Software Engineer",
                        ResumeContent = new Dictionary<string, string>
                        {
                            {"FileType", ".Pdf"},
                            {"Bytes",Guid.NewGuid().ToString()}
                        },
                        Status = "Pending"
                    }
                };

                expected[0].ResumeID = await _systemUnderTest.CreateResumeAsync(expected[0]);
                expected[1].ResumeID = await _systemUnderTest.CreateResumeAsync(expected[1]);

                var actual = await _systemUnderTest.GetResumesAsync(accountId);

                expected.ToExpectedObject().ShouldMatch(actual);
            }

            [Fact]
            public async Task update_status_correctly_Test()
            {
                var accountId = await _systemUnderTest.CreateAccountAsync(new Account
                {
                    AccountAlias = Guid.NewGuid().ToString(),
                    EmailAddress = $"{Guid.NewGuid().ToString()}@email.com",
                    Authentication = new Authentication
                    {
                        Salt = Guid.NewGuid().ToString(),
                        HashedPassword = Guid.NewGuid().ToString()
                    }
                });

                string oldstatus = "old status from Repository";
                Resume newResume = new Resume
                {
                    AccountID = accountId,
                    ApplyDate = DateTime.Today,
                    CompanyName = "Amazon",
                    JobUrl = "amazon.com/job",
                    Position = "Software Engineer",
                    ResumeContent = new Dictionary<string, string>
                        {
                            {"FileType", ".Pdf"},
                            {"Bytes",Guid.NewGuid().ToString()}
                        },
                    Status = oldstatus
                };

                int newRId = await _systemUnderTest.CreateResumeAsync(newResume);
                Resume resume = await _systemUnderTest.GetResumeAsync(newRId);
                Assert.Equal(oldstatus, resume.Status);

                string updateStatus = "update status from reporsitory";
                ResumeStorage rs = new ResumeStorage
                {
                    ResumeID = newRId,
                    status = updateStatus,
                };
                await _systemUnderTest.UpdateResume(rs);
                Resume updateResume = await _systemUnderTest.GetResumeAsync(newRId);
                Assert.NotEqual(oldstatus, updateResume.Status);
                Assert.Equal(updateStatus, updateResume.Status);



                //Assert.Equal(old)

                //var actual = await _systemUnderTest.GetResumesAsync(accountId);

                //expected.ToExpectedObject().ShouldMatch(actual);
            }
        }
    }
}
