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
        Task<Friends> AddFriendPairs(int myId, int theyId, string myStatus);
        Task<List<Friends>> GetFriendEntityByAccount(int AId1, int AId2);
        //Task<Friends> GetFriendEntityByFriendId(int friendId);
        Task<Friends> UpdateFriendPairStatus(int AId1, int AId2, string newStatus1, string newStatus2);
        Task<bool> DeleteFriendPairs(int myId, int theyId);
        Task<List<FriendInfo>> AllMyFriendPairs(int myId, string fStatus);
        Task<List<FriendInfo>> SearchUsers(string nameOrEmail, int meId);

        // Messages
        Task<string> AddNewMessage(int sendId, int receiveId, string newMsg);
        Task<bool> DeleteMessage(int messageId);
        Task<List<Message>> GetAllPersonallyMessages(int myId, int theyId);
        Task<List<FriendInfo>> GetAllTalkedAccount(int aId);


    }
}
