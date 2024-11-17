using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using ResumeRocketQuery.DataLayer;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;

namespace ResumeRocketQuery.DataLayerIntegrationTests
{
    
    [Rollback]
    public class ChatDataLayerTest
    {
        private IServiceProvider _serviceProvider;
        private IAccountDataLayer _accountDataLayer;

        public ChatDataLayerTest()
        {
            _serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _accountDataLayer = _serviceProvider.GetService<IAccountDataLayer>();
        }

        private IChatDateLayer GetSystemUnderTest(Type storageType)
        {
            return _serviceProvider.GetService<IChatDateLayer>();

        }


        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Stored_Friends_Pairs_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);
            
            var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = $"{Guid.NewGuid().ToString()}_YY_Test",
                LastName = $"{Guid.NewGuid().ToString()}_YY_Test",
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });


            var results = await systemUnderTest.AddFriendPairs(1438, accountId, "pending");

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            Assert.IsType<int>(results);
        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_add_new_Friends_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var secondStore = await systemUnderTest.AddFriendPairs(7041, 1439, "pending");

            Assert.NotNull(secondStore);
            // Assert.Equal(1, secondStore);
            // var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_respond_New_Friend_Request_Async(Type storagetype)
        {
            var systemUnderTest = GetSystemUnderTest(storagetype);
            var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = $"{Guid.NewGuid().ToString()}_YY_Test",
                LastName = $"{Guid.NewGuid().ToString()}_YY_Test",
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });

            var requestF = await systemUnderTest.AddFriendPairs(7041, accountId, "pending");
            Assert.NotNull(requestF);

            var result = await systemUnderTest.UpdateFriendPairStatus(accountId, 7041, "friends", "friends");
            Assert.NotNull(result);
            Assert.Equal("friends", result.Status);
            //var resultsdict = await systemundertest.allmyfriendpairs(1438, "friends");
        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Update_Friendship_Status_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var myFShip = await systemUnderTest.UpdateFriendPairStatus(1451, 7041, "pending", "unaccept");
            Assert.Equal("pending", myFShip.Status );

            //var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            //{
            //    AccountAlias = Guid.NewGuid().ToString(),
            //    FirstName = $"{Guid.NewGuid().ToString()}_YY_Test",
            //    LastName = $"{Guid.NewGuid().ToString()}_YY_Test",
            //    ProfilePhotoLink = Guid.NewGuid().ToString(),
            //    Title = Guid.NewGuid().ToString(),
            //    StateLocation = Guid.NewGuid().ToString(),
            //    PortfolioLink = Guid.NewGuid().ToString(),
            //});


            //var acceptFriends = await systemUnderTest.UpdateFriendPairStatus(fId, "friends");

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            //Assert.NotNull(acceptFriends);
            
            //var blockFriends = await systemUnderTest.UpdateFriendPairStatus(fId, "block");
            //Assert.Equal("block", blockFriends.Status);

        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Delete_Friendship_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var accountId = await _accountDataLayer.InsertAccountStorageAsync(new AccountStorage
            {
                AccountAlias = Guid.NewGuid().ToString(),
                FirstName = $"{Guid.NewGuid().ToString()}_YY_Test",
                LastName = $"{Guid.NewGuid().ToString()}_YY_Test",
                ProfilePhotoLink = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                StateLocation = Guid.NewGuid().ToString(),
                PortfolioLink = Guid.NewGuid().ToString(),
            });

            // delete the pair that is not exist 
            var deleteResultF = await systemUnderTest.DeleteFriendPairs(7041, accountId);
            Assert.False(deleteResultF);

            var added = await systemUnderTest.AddFriendPairs(7041, accountId, "pending");
            var deleteResultS = await systemUnderTest.DeleteFriendPairs(7041, accountId);
            Assert.True(deleteResultS);
            //var acceptFriends = await systemUnderTest.UpdateFriendPairStatus(fId, "friends");

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            //Assert.NotNull(acceptFriends);
            //Assert.Equal("friends", acceptFriends.Status);

            //var blockFriends = await systemUnderTest.UpdateFriendPairStatus(fId, "block");
            //Assert.Equal("block", blockFriends.Status);

        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Search_My_Friends_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            // check block person
            var blockPerson = await systemUnderTest.AllMyFriendPairs(7041, "block");
            Assert.NotNull(blockPerson);
            Assert.Equal(1451, blockPerson[0].AccountId);
            Assert.Equal("Ava", blockPerson[0].FirstName);

            //check friends
            var Friends = await systemUnderTest.AllMyFriendPairs(7041, "friends");
            Assert.NotNull(Friends);
            Assert.True(Friends.Count > 0);
            Assert.Equal("Mia", Friends[0].FirstName);


        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Search_All_Talked_People_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);


            var Friends = await systemUnderTest.GetAllTalkedAccount(1718);

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            Assert.NotNull(Friends);
            Assert.True(Friends.Count == 1);
            Assert.Equal("Yinghua", Friends[0].FirstName);
        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Search_personal_conversion_history_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var messages7041 = await systemUnderTest.GetAllPersonallyMessages(7041, 1448);

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            Assert.NotNull(messages7041);
            Assert.True(messages7041.Count >0);
            var messages1448 = await systemUnderTest.GetAllPersonallyMessages(1448, 7041);
            Assert.NotNull(messages7041);
            Assert.True(messages7041.Count == messages1448.Count);
        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Add_New_Msg_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var newMsg = await systemUnderTest.AddNewMessage(7041, 1448, "testMsg");

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            Assert.NotNull(newMsg);
            Assert.Equal("",newMsg);

            // send msg to block people
            var blockMsg = await systemUnderTest.AddNewMessage(1451, 7041, "hi");
            Assert.NotNull(blockMsg);
            Assert.Equal("block friends cannot send messages", blockMsg);
        }
        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Search_User_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var NameSearchResult = await systemUnderTest.SearchUsers("Mia Thromas", 7041);

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            Assert.NotNull(NameSearchResult);

            // send msg to block people
            var EmailSearchResult = await systemUnderTest.SearchUsers("Michael_Johnston36@hotmail.com", 7041);
            Assert.NotNull(EmailSearchResult);

            var FriendsSearchResult = await systemUnderTest.SearchUsers("", 7041);

        }
    }
}
