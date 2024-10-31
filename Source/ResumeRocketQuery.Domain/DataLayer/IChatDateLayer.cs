using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Domain.DataLayer
{
    /// <summary>
    /// This datalayer is using both Messages and Friendship table in database
    /// </summary>
    public interface IChatDateLayer
    {
        // Friendship table
        Task<int> AddFriendPairs(int myId, int newFriendId, string myStatus, string requestMsg = "");
        Task<Friends> GetFriendEntityByAccount(int AId1, int AId2);
        Task<Friends> GetFriendEntityByFriendId(int friendId);
        Task<Friends> UpdateFriendPairStatus(int friendId, string newStatus);
        Task<Friends> deleteFriendPairs(int myId, int friendId);
        Task<Dictionary<string, List<Friends>>> AllMyFriendPairs(int myId, string fStatus);

        // Messages
        Task<List<Message>> AddNewMessage(int sendId, int receiveId, string newMsg);


    }
}
