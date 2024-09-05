using ResumeRocketQuery.Domain.Api.Response;
using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Services
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountAlias { get; set; }
        public string EmailAddress { get; set; }
        public Authentication Authentication { get; set; }

        public string Name { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string PortfolioLink { get; set; }
        public string Resume { get; set; }
        public List<string> Skills { get; set; }
        public List<Experience> Experience { get; set; }
        public List<Education> Education { get; set; }
    }
}
