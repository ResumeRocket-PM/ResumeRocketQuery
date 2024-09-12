using System;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class ExperienceRequest
    {
        public string Company { get; set; }
        public string Position { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}


