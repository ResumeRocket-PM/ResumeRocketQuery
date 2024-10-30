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
        Task<int> AddFriendPairs(int myId, int newFriendId);
        Task<Friends> updateFriendPairStatus(int friendId, string newStatus);
        Task<Friends> deleteFriendPairs(int myId, int friendId);
        Task<Dictionary<string, List<Friends>>> AllMyFriendPairs(int myId, string fStatus);

    }
}
