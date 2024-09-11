using System;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class Experience
    {
        public int ExperienceId { get; set; }
        public int AccountId { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
