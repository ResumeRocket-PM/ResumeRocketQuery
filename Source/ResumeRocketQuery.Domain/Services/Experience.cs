using ResumeRocketQuery.Domain.Api.Response;
using System;
using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Services
{
    public class Experience
    {
        public string Company { get; set; }
        public string Position { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
