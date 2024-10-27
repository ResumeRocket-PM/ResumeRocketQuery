using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public class Change()
    {
        public string explanation
        {
            get;
            set;
        }
        public string original
        {
            get;
            set;
        }
        public string modified
        {
            get;
            set;
        }
    }
}
