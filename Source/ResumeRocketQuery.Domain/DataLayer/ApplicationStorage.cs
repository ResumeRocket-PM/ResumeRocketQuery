using System;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public class ApplicationStorage
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
