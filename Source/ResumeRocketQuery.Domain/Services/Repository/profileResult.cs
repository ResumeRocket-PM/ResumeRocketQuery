using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class ProfileResult
    {
        //State
        public string StatesName { get; set; }
        public string StateCode { get; set; }
        public string StateAbbr { get; set; }
        public int StatesID { get; set; }
        public string StateArea {  get; set; }

        //University
        public string UniversityName { get; set; }

        public string Area { get; set; }

        public string Country {  get; set; }

        // Major
        public string MajorName { get; set; }

        // Career
        public string CareerName { get; set; }
    }
}
