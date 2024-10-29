using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class Friends
    {
        public int FriendId { get; set; }
        public int AccountId1 { get; set; }
        public int AccountId2 { get; set;}
        public string Status { get; set; }
        public string CreatedTime {  get; set; }

    }
}
