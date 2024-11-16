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
        public async Task<Friends> RequestNewFriends(int myId, int newFId)
        {
            var result  = await _chatDataLayer.AddFriendPairs(myId, newFId, "pending");
            return result;
        }

        // accept/reject friends
        public async Task<Friends> RespondNewFriends(int meId, int theyId, string updateStatus)
        {
            if (updateStatus == "accept")
            {
                var result = await _chatDataLayer.UpdateFriendPairStatus(meId, theyId, "friends", "friends");
                return result;

            }
            else if (updateStatus == "blocking")
            {
                var result = await _chatDataLayer.UpdateFriendPairStatus(meId, theyId, "unfriends", "unfriends");
                return result;
            }
            else
            {
                var result = await _chatDataLayer.UpdateFriendPairStatus(meId, theyId, "reject", "pending");
                return result;
            }
        }

        /// <summary>
        /// this search function can take the input as protofilo link, Name, and email
        /// </summary>
        /// <param name="meId"></param>
        /// <param name="searchInput"></param>
        /// <returns></returns>
        public async Task<List<FriendInfo>> searchUserAccount(int meId, string searchInput)
        {
            var result = await _chatDataLayer.SearchUsers(searchInput, meId);
            return result;
        }

        /// <summary>
        /// show all of my friends in different status
        /// the status should be in three different value:
        /// (1): friends
        /// (2): unaccept
        /// (3): block
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<FriendInfo>> ShowFriendsListWithStatus(int myId, string status)
        {
            var result = await _chatDataLayer.AllMyFriendPairs(myId, status);
            return result;
        }

        /// <summary>
        /// add the new message into the Database, the status should be 'unread'
        /// </summary>
        /// <param name="sendId"></param>
        /// <param name="receiveId"></param>
        /// <param name="newMsg"></param>
        /// <returns></returns>
        public async Task<string> SendMsg(int sendId, int receiveId, string newMsg)
        {
            var result = await _chatDataLayer.AddNewMessage(sendId, receiveId, newMsg);
            return result;
        }

        /// <summary>
        /// get personal conversion message history by accountId pair,
        /// and return the list of message in descending order.
        /// 
        /// This currently can only take recent 200 messsages,
        /// and the first on from the return list should be the latest message
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="theyId"></param>
        /// <returns></returns>
        public async Task<List<Message>> GetMessageHistory(int myId, int theyId)
        {
            var result = await _chatDataLayer.GetAllPersonallyMessages(myId, theyId);  
            return result;
        }
        
        /// <summary>
        /// Get All talked people account info.
        /// </summary>
        /// <param name="acccountId"></param>
        /// <returns></returns>
        public async Task<List<FriendInfo>> GetTalkedPeople(int acccountId)
        {
            var result = await _chatDataLayer.GetAllTalkedAccount(acccountId);
            return result;
        }

//--------------------finished that later------------------------
        // delete friends

        // block friends
    }
}
