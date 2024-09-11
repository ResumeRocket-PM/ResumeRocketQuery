using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class AccountDetails
    {
        public int AccountId { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhotoLink { get; set; }
        public string Title { get; set; }
        public string StateLocation { get; set; }
        public string PortfolioLink { get; set; }
        public List<string> Skills { get; set; }
        public List<Experience> Experience { get; set; }
        public List<Education> Education { get; set; }
    }
}
