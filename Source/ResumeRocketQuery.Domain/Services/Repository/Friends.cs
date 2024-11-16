using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class Friends
    {
        public int FriendsId { get; set; }
        public int AccountId1 { get; set; }
        public int AccountId2 { get; set;}
        public string Status { get; set; }
        public string CreatedTime {  get; set; }

        /// <summary>
        /// this is for some querys that return a query result
        /// to say if the query is executed success or not
        /// </summary>
        public string Result { get; set; }
    }
}
