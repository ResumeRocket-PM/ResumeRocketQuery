using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Services
{
    public class ChatService: IChatService
    {
        private readonly IChatDateLayer _chatDataLayer;
        public ChatService(IChatDateLayer chatDataLayer)
        {
            _chatDataLayer = chatDataLayer;
        }
        
        /// <summary>
        /// Given my account Id, and the new friend Account Id, and the request message like 
        /// short msg of introducing myself, and then return the friendsId
        /// 
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="newFId"></param>
        /// <param name="requestMsg"></param>
        /// <returns></returns>
        public async Task<int> RequestNewFriends(int myId, int newFId, string requestMsg)
        {
            var result  = await _chatDataLayer.AddFriendPairs(myId, newFId, "pending", requestMsg);
            return result;
        }

        // accept/reject friends
        public async Task<Friends> RespondNewFriends(int FId, string updateStatus)
        {
            var result = await _chatDataLayer.UpdateFriendPairStatus(FId, updateStatus);
            return result;
        }

        // show friends list
        public async Task<List<FriendInfo>> ShowFriendsListWithStatus(int myId, string status)
        {
            var result = await _chatDataLayer.AllMyFriendPairs(myId, status);
            return result;
        }

        // send message
        public async Task<List<Message>> SendMsg(int sendId, int receiveId, string newMsg)
        {
            var result = await _chatDataLayer.AddNewMessage(sendId, receiveId, newMsg);
            return result;
        }

        // show messages
        public async Task<List<Message>> GetMessageHistory(int Fid)
        {
            var result = await _chatDataLayer.GetAllMessages(Fid);  
            return result;
        }


//--------------------finished that later------------------------
        // delete friends

        // block friends
    }
}
