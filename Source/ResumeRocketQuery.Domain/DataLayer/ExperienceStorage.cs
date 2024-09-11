using System;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public class ExperienceStorage
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
