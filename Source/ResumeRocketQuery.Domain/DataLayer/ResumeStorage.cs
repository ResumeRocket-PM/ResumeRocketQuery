using System;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public class ResumeStorage
    {
        public int ResumeID { get; set; }
        public DateTime applyDate { get; set; }
        public string jobUrl { get; set; }
        public int accountID { get; set; }
        public string status { get; set; }
        public string resume { get; set; }
        public string position { get; set; }
        public string companyName { get; set; }
    }
}
