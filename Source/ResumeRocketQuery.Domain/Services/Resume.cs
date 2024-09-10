using System;
using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Services
{
    public class Resume
    {
        public int ResumeID { get; set; }
        public DateTime? ApplyDate { get; set; }
        public string JobUrl { get; set; }
        public int AccountID { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> ResumeContent { get; set; }
        public string Position { get; set; }
        public string CompanyName { get; set; }
    }
}
