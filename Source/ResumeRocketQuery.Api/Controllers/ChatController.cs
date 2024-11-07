using Microsoft.AspNetCore.Mvc;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Sprache;

namespace ResumeRocketQuery.Api.Controllers
{
    [Authorize]
    [Route("api/Chat")]
    public class ChatController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly IChatService _chatService;
        public ChatController(IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder,
            IChatService chatService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _chatService = chatService;
        }

        [HttpPost]
        [Route("requestFriend")]
        public async Task<ServiceResponseGeneric<int>> requestFriends([FromBody] FriendsRequest request)
        {
            var result = await _chatService.RequestNewFriends(request.requestId, request.acceptId, request.acceptReason);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("respondNewFriend/{FriendsId}/{newStatus}")]
        public async Task<ServiceResponseGeneric<Friends>> ReplyFriendRequest([FromRoute] int FriendsId, [FromRoute] string newStatus)
        {
            var result = await _chatService.RespondNewFriends(FriendsId, newStatus);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("sendMsg")]
        public async Task<ServiceResponseGeneric<List<Message>>> SendMessage([FromBody] SendMsgRequest request)
        {
            var result = await _chatService.SendMsg(request.SendId, request.ReceiveId, request.Msg);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);

        }

        [HttpGet]
        [Route("MsgHistory/{fId}")]
        public async Task<ServiceResponseGeneric<List<Message>>> GetChatHistory([FromBody] int fId)
        {
            var result = await _chatService.GetMessageHistory(fId);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);

        }

        [HttpGet]
        [Route("friendsList/{status}")]
        public async Task<ServiceResponseGeneric<List<FriendInfo>>> GetFriendsList([FromRoute] string status)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);
            var result = await _chatService.ShowFriendsListWithStatus(user.AccountId, status);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);

        }
    }
}
