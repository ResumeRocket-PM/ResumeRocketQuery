using System.Collections.Generic;
using System;

namespace ResumeRocketQuery.Domain.Api.Response
{
    public class Education
    {
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public DateTime GraduationDate { get; set; }
        public List<string> Courses { get; set; }
    }
}

