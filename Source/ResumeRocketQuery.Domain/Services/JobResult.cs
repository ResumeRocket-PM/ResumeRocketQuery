using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public class JobResult
    {
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Perks { get; set; }
        public List<string> Requirements { get; set; }
    }
}
