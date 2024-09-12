using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public class Updates()
    {
        public string html
        {
            get;
            set;
        }
        public string css
        {
            get;
            set;
        }
        public List<Change> changes
        {
            get;
            set;
        }
    }
    public class Change()
    {
        public string section
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
