using System;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public class ResumeStorage
    {
        public int ResumeId { get; set; }
        public int AccountId { get; set; }
        public string Resume { get; set; }
    }
}
