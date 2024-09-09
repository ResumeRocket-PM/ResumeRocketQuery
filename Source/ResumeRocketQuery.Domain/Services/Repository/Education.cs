using System;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class Education
    {
        public int EducationId { get; set; }
        public int AccountId { get; set; }
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public DateTime? GraduationDate { get; set; }
    }
}
