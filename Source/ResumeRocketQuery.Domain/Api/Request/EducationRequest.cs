using System;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class EducationRequest
    {
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public DateTime? GraduationDate { get; set; }
    }
}


