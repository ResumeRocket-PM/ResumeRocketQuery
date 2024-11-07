using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class FriendsRequest
    {
        public int requestId{ get; set; }
        public int acceptId { get; set; }
        public string acceptStatus { get; set; }
        public string acceptReason { get; set; }
    }
}
