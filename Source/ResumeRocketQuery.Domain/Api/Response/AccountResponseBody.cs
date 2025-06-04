using System.Collections.Generic;
using System;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Domain.Api.Response
{
    public class AccountResponseBody
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhotoLink { get; set; }
        public string BackgroundPhotoLink { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string PortfolioLink { get; set; }
        public int? PrimaryResumeId { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Experience> Experience { get; set; }
        public List<Education> Education { get; set; }

    }
}

