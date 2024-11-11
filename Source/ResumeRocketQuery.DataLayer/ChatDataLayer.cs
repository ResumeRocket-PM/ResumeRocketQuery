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
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using OpenQA.Selenium.DevTools.V126.Console;
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
        public async Task<int> AddFriendPairs(int myId, int newFriendId, string myStatus = "pending", string requestMsg = "")
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
                                    status = myStatus
                                },
                                commandType: CommandType.Text);

                    // add the request message into the Messages Table
                    if (requestMsg.Length > 0)
                    {
                        var messageResult = await connection.QueryAsync<Message>(
                                DataLayerConstants.StoredProcedures.Chat.AddMsgByAId,
                                new
                                {
                                    fId = FriendshipResult,
                                    sId = myId,
                                    rId = newFriendId,
                                    content = requestMsg,
                                    status = myStatus
                                },
                                commandType: CommandType.Text);
                    }

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
        public async Task<List<FriendInfo>> AllMyFriendPairs(int myId, string fStatus)
        {
            // find all the AccountId1 or AccountId2 that is same with the @myId
            // (*Notice: the Id might in AccountId1 or AccountId2*)
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<FriendInfo>(
                            DataLayerConstants.StoredProcedures.Chat.FindAllFrinedsByAccountId,
                            new
                            {
                                accountId = myId,
                                status = fStatus
                            },
                            commandType: CommandType.Text);

                return result.ToList();

            }

        }

//_______________________________________________Messages_____________________________________________________
        // add new messages and show the top 10 latest messages recording
        public async Task<string> AddNewMessage(int sId, int rId, string newMsg)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                //get friendsId if there is 
                var Friends = await connection.QueryAsync<Friends>(
                            DataLayerConstants.StoredProcedures.Chat.GetFriendsByAccount,
                            new
                            {
                                accountId1 = sId,
                                accountId2 = rId
                            },
                            commandType: CommandType.Text);
                
                if (Friends == null) // if there is no friendsId, then just add the message
                {
                    var result = await connection.ExecuteScalarAsync<int>(
                            DataLayerConstants.StoredProcedures.Chat.AddMessageEntity,
                            new
                            {
                                sendId = sId,
                                receiveId = rId,
                                text = newMsg,
                                status = "unread"
                            },
                            commandType: CommandType.Text);
                    return "";
                }
                else // check if the friends status is block 
                {
                    var FriendsTuple = Friends.ToList();
                    bool isBlock = FriendsTuple[0].Status == "block";
                    if (isBlock)
                    {
                        return "block friends cannot send messages";
                    }
                    else
                    {
                        var result = await connection.ExecuteScalarAsync<int>(
                            DataLayerConstants.StoredProcedures.Chat.AddMessageEntity,
                            new
                            {
                                sendId = sId,
                                receiveId = rId,
                                text = newMsg,
                                status = "unread"
                            },
                            commandType: CommandType.Text);
                        return "";
                    }
                }
            }
        }
        // delete messages
        public async Task<bool> DeleteMessage(int messageId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                DateTime now = DateTime.UtcNow;
                var msg = await connection.QueryFirstOrDefaultAsync<Message>(
                    DataLayerConstants.StoredProcedures.Chat.GetMsgbyMsgId,
                    new
                    {
                        msgId = messageId
                    }, commandType: CommandType.Text);
                DateTime msgTime;
                var timeResult = DateTime.TryParse(msg.MsgTime, out msgTime);
                TimeSpan timeDifference = now - msgTime;
                if (timeDifference.TotalMinutes > 5)
                {
                    return false;
                }
                else
                {
                    var msgList = await connection.QueryAsync<Message>(
                    DataLayerConstants.StoredProcedures.Chat.DeleteMsg,
                    new
                    {
                        msgId = messageId
                    }, commandType: CommandType.Text);
                    return true;
                }
            }
        }
        
        /// <summary>
        /// get the personal coversation message history,
        /// this currently can only take recent 200 messsages,
        /// and the first on from the return list should be the latest message
        /// 
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="theyId"></param>
        /// <returns></returns>
        public async Task<List<Message>> GetAllPersonallyMessages(int myId, int theyId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var msgList = await connection.QueryAsync<Message>(
                    DataLayerConstants.StoredProcedures.Chat.GetAllMsgContent,
                    new 
                    {
                        aId1 = myId,
                        aId2 = theyId
                    }, commandType: CommandType.Text);
                List<Message> messageList = msgList.ToList();
                foreach(var message in messageList)
                {
                    if(message.SendId == myId)
                    {
                        message.Identity = "me";
                    }
                    else
                    {
                        message.Identity = "they";
                    }
                }
                return messageList;
            }
        }

        /// <summary>
        /// show all talked people in unique
        /// </summary>
        /// <param name="aId"></param>
        /// <returns></returns>
        public async Task<List<FriendInfo>> GetAllTalkedAccount(int aId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var msgList = await connection.QueryAsync<FriendInfo>(
                    DataLayerConstants.StoredProcedures.Chat.GetAlltalkedAccounts,
                    new
                    {
                        accountId = aId
                    }, commandType: CommandType.Text);
                return msgList.ToList();

            }
        }
    }
}
