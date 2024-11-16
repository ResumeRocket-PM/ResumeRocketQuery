using ResumeRocketQuery.Domain.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IChatService
    {
        Task<Friends> RequestNewFriends(int myId, int theyId);
        Task<Friends> RespondNewFriends(int myId, int theyId, string respondRequest);
        Task<List<FriendInfo>> ShowFriendsListWithStatus(int myId, string status);
        Task<string> SendMsg(int sendId, int receiveId, string newMsg);
        Task<List<Message>> GetMessageHistory(int myId, int theyId);
        Task<List<FriendInfo>> GetTalkedPeople(int acccountId);
        Task<List<FriendInfo>> searchUserAccount(int meId, string searchInput);

    }
}
