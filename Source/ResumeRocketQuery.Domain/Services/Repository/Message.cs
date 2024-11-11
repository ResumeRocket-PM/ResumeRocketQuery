using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class Message
    {
        public int MsgId {  get; set; }
        //public int FriendId { get; set; }
        public int SendId {  get; set; }
        public int ReceiveId { get; set; }
        public string MsgContent { get; set; }
        public string MsgTime { get; set; }
        public string MsgStatus {  get; set; }
        public string Identity { get; set; }
    }
}
