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
    /// <summary>
    /// <Start> Oct. 18th, 2024 --Yinghua Yin
    ///     
    ///     <Form> Database samples
    ///        FriendsId	AccountId1	AccountId2	Status	    CreatedTime
    ///        1	        1438	    1439	    friends	    2024-10-27 15:53:15.160
    ///        2	        1438	    1440	    friends	    2024-10-27 15:54:21.520
    ///        3	        1438	    1441	    friends	    2024-10-27 15:54:21.520
    ///        4	        1438	    1442	    friends	    2024-10-27 15:54:21.520
    ///        5	        1438	    1443	    friends	    2024-10-27 15:54:21.523
    ///        15	        1438	    1445	    friends	    2024-10-27 16:26:31.810
    ///        16	        1438	    1446	    friends	    2024-10-27 16:37:39.113
    ///        17	        1438	    1447	    friends	    2024-10-27 16:37:53.437
    ///        18	        1438	    1448	    friends	    2024-10-27 16:37:59.523
    ///        20	        1438	    1449	    pending	    2024-10-27 16:45:44.123
    ///        23	        1718	    7041	    pending	    2024-10-27 21:33:53.390
    ///     </Form>
    ///     
    /// </Start>
    /// 
    /// </summary>
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


            var results = await systemUnderTest.AddFriendPairs(1438, accountId);

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            Assert.IsType<int>(results);
        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Stored_Friends_Pairs_Duplicate_Async(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var secondStore = await systemUnderTest.AddFriendPairs(1438, 1439);

            Assert.Equal(1, secondStore);
            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task GetFriendsByFId(Type storageType)
        {
            var systemUnderTest = GetSystemUnderTest(storageType);

            var friendPair = await systemUnderTest.GetFriendEntityByFriendId(2);

            Assert.Equal(1438, friendPair.AccountId1);
            Assert.Equal(1440, friendPair.AccountId2);

            // 
            var nan = await systemUnderTest.GetFriendEntityByFriendId(-1);
            Assert.Null(nan);
            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
        }

        [Theory]
        [InlineData(typeof(ChatDataLayer))]
        public async Task Check_Update_Friendship_Status_Async(Type storageType)
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


            var fId = await systemUnderTest.AddFriendPairs(1438, accountId);
            var acceptFriends = await systemUnderTest.UpdateFriendPairStatus(fId, "friends");

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            Assert.NotNull(acceptFriends);
            Assert.Equal("friends", acceptFriends.Status );
            
            var blockFriends = await systemUnderTest.UpdateFriendPairStatus(fId, "block");
            Assert.Equal("block", blockFriends.Status);

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


            var fId = await systemUnderTest.AddFriendPairs(1438, accountId);
            var acceptFriends = await systemUnderTest.UpdateFriendPairStatus(fId, "friends");

            //var resultsDict = await systemUnderTest.AllMyFriendPairs(1438, "friends");
            Assert.NotNull(acceptFriends);
            Assert.Equal("friends", acceptFriends.Status);

            var blockFriends = await systemUnderTest.UpdateFriendPairStatus(fId, "block");
            Assert.Equal("block", blockFriends.Status);

        }

    }
}
