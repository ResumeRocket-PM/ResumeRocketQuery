using System;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public class JobPosting
    {
        public int JobId { get; set; }
        public string JobUrl { get; set; }
        public string JobCompany { get; set; }
        public string JobDescription { get; set; }
    }
}