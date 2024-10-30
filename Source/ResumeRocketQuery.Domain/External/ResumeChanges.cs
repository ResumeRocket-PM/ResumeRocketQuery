using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public class Change
    {
        public string OriginalText { get; set;}
        public string ModifiedText { get; set; }
        public string Explanation { get; set; }
        public string DivClass { get; set; }
    }
}
