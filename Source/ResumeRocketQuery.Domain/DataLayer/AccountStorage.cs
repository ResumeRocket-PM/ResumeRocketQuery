using System;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public class AccountStorage
    {
        public int AccountId { get; set; }
        public string AccountAlias { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhotoLink { get; set; }
        public string Title { get; set; }
        public string StateLocation { get; set; }
        public string PortfolioLink { get; set; }
        public int PrimaryResumeId { get; set; }
    }
}
