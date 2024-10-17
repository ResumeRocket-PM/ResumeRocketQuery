using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Services
{
    public class ResumeRequest
    {
        public int AccountId { get; set; }
        public Dictionary<string, string> Pdf { get; set; }
        public int? OriginalResumeID { get; set; }
        public int? ResumeId { get; set; }

    }
}
