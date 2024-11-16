using ResumeRocketQuery.Domain.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Api.Response
{
    public  class ExtensionPostingsResponse
    {
        public int ApplicationID { get; set; }
        public List<Change> Suggestions { get; set; }
    }

    public class Change
    {
        public string explanation { get; set; }
        public string original { get; set; }
        public string modified { get; set; }
    }
}
