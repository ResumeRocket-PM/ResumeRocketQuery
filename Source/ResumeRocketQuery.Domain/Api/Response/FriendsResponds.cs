using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Api.Response
{
    public class FriendsResponds
    {
        public int FriendsId {  get; set; }
        public int AccountId1 {  get; set; }
        public int AccountId2 { get;  set; }

        public string Status { get; set; }
        public string CreateTime { get; set; }

    }
}
