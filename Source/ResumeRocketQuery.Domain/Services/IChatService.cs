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
        Task<int> RequestNewFriends(int myId, int newFId, string requestMsg);
        Task<Friends> RespondNewFriends(int FId, string updateStatus);
        Task<List<FriendInfo>> ShowFriendsListWithStatus(int myId, string status);
        Task<List<Message>> SendMsg(int sendId, int receiveId, string newMsg);
        Task<List<Message>> GetMessageHistory(int Fid);

    }
}
