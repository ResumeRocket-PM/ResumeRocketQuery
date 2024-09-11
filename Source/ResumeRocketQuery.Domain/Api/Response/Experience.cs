using System.Collections.Generic;
using System;

namespace ResumeRocketQuery.Domain.Api.Response
{
    public class Experience
    {
        public string Company { get; set; }
        public string Position { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

