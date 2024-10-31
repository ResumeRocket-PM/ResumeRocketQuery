using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Data;
using System.Security.Principal;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Domain.DataLayer;
using System.ComponentModel;
namespace ResumeRocketQuery.DataLayer
{
    /// <summary>
    /// This datalayer is using both Messages and Friendship table in database
    /// </summary>
    public class ChatDataLayer : IChatDateLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public ChatDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }

        /// <summary>
        /// add the new friend pairs int to Friendship table in Database
        /// the Frinedship pairs(AccountId1 and AccountId2) shhould be unique
        /// and the AccountId1(Int) should be less than AccountId2(Int) 
        /// 
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="newFriendId"></param>
        /// <returns></returns>
        public async Task<int> AddFriendPairs(int myId, int newFriendId, string requestMsg = "")
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {

                try
                {
                    // check if there is friend pair
                    // if there is no friend pair,
                    // just added, and set the status as pending, return friendId
                    var FriendshipResult = await connection.ExecuteScalarAsync<int>(
                                DataLayerConstants.StoredProcedures.Chat.AddNewFriends,
                                new
                                {
                                    inputId1 = myId,
                                    inputId2 = newFriendId,
                                    status = "pending"
                                },
                                commandType: CommandType.Text);

                    return FriendshipResult;

                    // adding in the message as well
                }
                catch (SqlException)
                {
                    // if friend pair exist, and the friend status is pending/block
                    // pending: return friendId

                    // block: do nothing

                    Friends existFriends = await GetFriendEntityByAccount(myId, newFriendId);

                    if (existFriends != null)
                    {
                        return existFriends.FriendsId;
                    }
                    else
                    {
                        return -1;
                    }

                }

            }
        }
        
        public async Task<Friends> GetFriendEntityByAccount(int AId1, int AId2)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<Friends>(
                                        DataLayerConstants.StoredProcedures.Chat.GetFriendsByAccount,
                                        new
                                        {
                                            inputId1 = AId1,
                                            inputId2 = AId2,
                                        },
                                        commandType: CommandType.Text);

                return result; 
            }
        }

        /// <summary>
        /// get the specific entity from Friendship table by its friendsId
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task<Friends> GetFriendEntityByFriendId(int friendId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<Friends>(
                                        DataLayerConstants.StoredProcedures.Chat.GetFriendPairs,
                                        new
                                        {
                                            friendsId = friendId
                                        },
                                        commandType: CommandType.Text);

                return result;
            }
        }

        /// <summary>
        /// change the status of friend paris by the friendsId
        /// </summary>
        /// <param name="friendId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task<Friends> UpdateFriendPairStatus(int friendId, string newStatus)
        {
            // get the raw of friend pairs, and set the status as the value of @status
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {

                var result = await connection.QueryFirstOrDefaultAsync<Friends>(
                    DataLayerConstants.StoredProcedures.Chat.UpdateFriendStatus,
                    new
                    {
                        friendsId = friendId,
                        status = newStatus
                    },
                    commandType: CommandType.Text);

                return result;

            }
        }

        /// <summary>
        /// delete the Friendship entity from the Friendship table
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task<Friends> deleteFriendPairs(int myId, int friendId)
        {
            // get the raw of friend pairs, and set the status as the value of @status
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {

                var result = await connection.QueryFirstOrDefaultAsync<Friends>(
                    DataLayerConstants.StoredProcedures.Chat.UpdateFriendStatus,
                    new
                    {
                        inputAccountId1 = myId,
                        inputAccountId2 = friendId,
                    },
                    commandType: CommandType.Text);

                return result;
            }
        }

        // show friend list
        public async Task<Dictionary<string, List<Friends>>> AllMyFriendPairs(int myId, string fStatus)
        {
            // find all the AccountId1 or AccountId2 that is same with the @myId
            // (*Notice: the Id might in AccountId1 or AccountId2*)
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var List1 = await connection.QueryAsync<Friends>(
                            DataLayerConstants.StoredProcedures.Chat.findAllFriendsAccount1,
                            new
                            {
                                accountId = myId,
                                status = fStatus
                            },
                            commandType: CommandType.Text);

                var List2 = await connection.QueryAsync<Friends>(
                            DataLayerConstants.StoredProcedures.Chat.findAllFriendsAccount2,
                            new
                            {
                                accountId = myId,
                                status = fStatus
                            },
                            commandType: CommandType.Text);

                Dictionary<string, List<Friends>> result = new()
                {
                    {"me_is_accountId_1", List1.ToList() },
                    {"me_is_accountId_2", List2.ToList() }

                };
                return result;

            }

        }

//_______________________________________________Messages_____________________________________________________
        // add new messages and show the top 10 latest messages recording

        // delete messages

        //
    }
}
