using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public class ApplicationResult
    {
        public int ResumeID { get; set; }
        public DateTime? ApplyDate { get; set; }
        public string JobUrl { get; set; }
        public int AccountID { get; set; }
        public string Status { get; set; }
        public string ResumeContent { get; set; }
        public string Position { get; set; }
        public string CompanyName { get; set; }
        public int? ResumeContentId { get; set; }
    }
}
