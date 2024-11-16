using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class SendMsgRequest
    {
        public int SendId { get; set; }
        public int ReceiveId { get; set; }
        public string Msg { get; set; }
    }
}
