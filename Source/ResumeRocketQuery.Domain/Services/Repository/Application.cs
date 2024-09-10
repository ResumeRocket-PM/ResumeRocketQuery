using System;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public int AccountId { get; set; }
        public int? ResumeId { get; set; }
        public DateTime? ApplyDate { get; set; }
        public string Status { get; set; }
        public string Position { get; set; }
        public string CompanyName { get; set; }
        public string JobPostingUrl { get; set; }
    }
}
