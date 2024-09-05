using System.Collections.Generic;
using System;

namespace ResumeRocketQuery.Domain.Api.Response
{
    public class AccountResponseBody
    {
        public string Name { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string PortfolioLink { get; set; }
        public string Resume { get; set; }
        public List<string> Skills { get; set; }
        public List<Experience> Experience { get; set; }
        public List<Education> Education { get; set; }

    }
}

