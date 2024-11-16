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


        public async Task<Friends> AddFriendPairs(int myId, int theyId, string myStatus = "pending")
        {
            var friends = await GetFriendEntityByAccount(myId, theyId);
            if (friends.Count == 0)
            {
                using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
                {

                    var result = await connection.QueryFirstAsync<Friends>(
                                DataLayerConstants.StoredProcedures.Chat.AddNewFriends,
                                new
                                {
                                    accountId1 = myId,
                                    accountId2 = theyId,
                                    status1 = myStatus,
                                    status2 = "unaccept"
                                },
                                commandType: CommandType.Text);
                    return result;
                }
            }
            else
            {
                if (friends[0].Status == "unaccept") 
                {
                    var result = await UpdateFriendPairStatus(myId, theyId, "friends", "friends");
                    return result;
                }
                else if (friends[0].Status != "pending" && friends[0].Status != "friends")
                {
                    var result = await UpdateFriendPairStatus(myId, theyId, "pending", "unaccept");
                    return result;

                }
                else
                {
                    return friends[0];
                }
            }
        }
        
        public async Task<List<Friends>> GetFriendEntityByAccount(int AId1, int AId2)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<Friends>(
                                        DataLayerConstants.StoredProcedures.Chat.GetFriendsByAccount,
                                        new
                                        {
                                            accountId1 = AId1,
                                            accountId2 = AId2,
                                        },
                                        commandType: CommandType.Text);

                return result.ToList(); 
            }
        }

        /// <summary>
        /// change the status of friend paris by the friendsId
        /// </summary>
        /// <param name="friendId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task<Friends> UpdateFriendPairStatus(int AId1, int AId2, string newStatus1, string newStatus2)
        {
            // get the raw of friend pairs, and set the status as the value of @status
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryFirstAsync<Friends>(
                                        DataLayerConstants.StoredProcedures.Chat.UpdateFriendsStatus,
                                        new
                                        {
                                            accountId1 = AId1,
                                            accountId2 = AId2,
                                            status1 = newStatus1,
                                            status2 = newStatus2
                                        },
                                        commandType: CommandType.Text);
                
                return result;
                
            }
        }
        
        //public async Task<Friends> GetFriendEntityByFriendId(int friendId)
        //{
        //    using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
        //    {
        //        var result = await connection.QueryFirstOrDefaultAsync<Friends>(
        //                                DataLayerConstants.StoredProcedures.Chat.GetFriendPairs,
        //                                new
        //                                {
        //                                    friendsId = friendId
        //                                },
        //                                commandType: CommandType.Text);

        //        return result;
        //    }
        //}

        

        /// <summary>
        /// delete the Friendship entity from the Friendship table
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="friendId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFriendPairs(int myId, int theyId)
        {
            // get the raw of friend pairs, and set the status as the value of @status
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {

                var result = await connection.QueryFirstOrDefaultAsync<Friends>(
                    DataLayerConstants.StoredProcedures.Chat.DeleteFriendsPair,
                    new
                    {
                        accountId1 = myId,
                        accountId2 = theyId,
                    },
                    commandType: CommandType.Text);

                if (result != null)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// this should return all entities in Friendship table
        /// that is related to the myId, based on the given status(fStatus), 
        /// it will shows different result from Friendship table
        /// </summary>
        /// <param name="myId"></param>
        /// <param name="fStatus"></param>
        /// <returns></returns>
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
                                status = $"%{fStatus}%"
                            },
                            commandType: CommandType.Text);

                return result.ToList();

            }

        }

        /// <summary>
        /// searched other user based on the searchInput value
        /// this function could deal with the value:
        /// (1) Name, (2) email, (3) PortfolioLink
        /// 
        /// </summary>
        /// <param name="searchInput"></param>
        /// <param name="meId"></param>
        /// <returns></returns>
        public async Task<List<FriendInfo>> SearchUsers(string searchInput, int meId)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                if (searchInput.Contains('@'))
                {
                    var result = await connection.QueryAsync<FriendInfo>(
                            DataLayerConstants.StoredProcedures.Chat.SearchUserByEmail,
                            new
                            {
                                email = $"%{searchInput}%",
                                myId = meId
                            },
                            commandType: CommandType.Text);

                    return result.ToList();
                }
                else if (searchInput.Contains("https"))
                {
                    var result = await connection.QueryAsync<FriendInfo>(
                            DataLayerConstants.StoredProcedures.Chat.SearchUserByPortfolio,
                            new
                            {
                                userName = searchInput,
                                myId = meId
                            },
                            commandType: CommandType.Text);

                    return result.ToList();
                }
                else // search by name
                {
                    string[] fullName = searchInput.Split(' ');
                    var firstName = fullName[0];
                    var lastName = fullName.Length == 1 ? fullName[0] : fullName[1];
                    var result = await connection.QueryAsync<FriendInfo>(
                                DataLayerConstants.StoredProcedures.Chat.SearchUserByName,
                                new
                                {
                                    fName = $"%{firstName}%",
                                    lName = $"%{lastName}%",
                                    myId = meId
                                },
                                commandType: CommandType.Text);

                    return result.ToList();
                }
                

            }
        }
//_______________________________________________Messages_____________________________________________________
        /// <summary>
        /// add new message in the Database by given sendId(sId), rId(receiveId), Msg content(newMsg)
        /// 
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="rId"></param>
        /// <param name="newMsg"></param>
        /// <returns></returns>
        public async Task<string> AddNewMessage(int sId, int rId, string newMsg)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                //get friendsId if there is 
                var Friend = await connection.QueryAsync<Friends>(
                            DataLayerConstants.StoredProcedures.Chat.GetFriendsByAccount,
                            new
                            {
                                accountId1 = sId,
                                accountId2 = rId
                            },
                            commandType: CommandType.Text);
                var FriendsTuple = Friend.ToList();
                if (FriendsTuple.Count == 0) // if there is no friendsId, then just add the message
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
                    bool isBlock = (FriendsTuple[0].Status == "blocking") || (FriendsTuple[0].Status == "blocked");
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
