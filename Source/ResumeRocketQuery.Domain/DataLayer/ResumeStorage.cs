using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public class ResumeStorage
    {
        public int AccountId { get; set; }
        public string JobUrl { get; set; }
        public string Resume { get; set; }
    }
}
