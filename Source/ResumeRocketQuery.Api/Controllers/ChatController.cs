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
        private readonly IAccountService _accountService;
        public ChatController(IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder, 
            IAccountService accountService,
            IChatService chatService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _chatService = chatService;
            _accountService = accountService;


        }

        [HttpPost]
        [Route("requestFriend/{theyId}")]
        public async Task<ServiceResponseGeneric<Friends>> requestFriends([FromRoute] int theyId)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);
            var result = await _chatService.RequestNewFriends(user.AccountId, theyId);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("respondNewFriend/{theyId}/{respond}")]
        public async Task<ServiceResponseGeneric<Friends>> ReplyFriendRequest([FromRoute] int theyId, [FromRoute] string respond)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);

            var result = await _chatService.RespondNewFriends(user.AccountId, theyId, respond);

            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("sendMsg")]
        public async Task<ServiceResponseGeneric<string>> SendMessage([FromBody] Message msgRequest)
        {
            var sender = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);
            var result = await _chatService.SendMsg(sender.AccountId, msgRequest.ReceiveId, msgRequest.MsgContent);
            
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("MsgHistory/{theyId}")]
        public async Task<ServiceResponseGeneric<List<Message>>> GetChatHistory([FromRoute] int theyId)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);
            var result = await _chatService.GetMessageHistory(user.AccountId, theyId);
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

        [HttpGet]
        [Route("AllTalkedPeople")]
        public async Task<ServiceResponseGeneric<List<FriendInfo>>> GetFriendsList()
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);
            var result = await _chatService.GetTalkedPeople(user.AccountId);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);

        }

        [HttpGet]
        [Route("searchUser/{input}")]
        public async Task<ServiceResponseGeneric<List<FriendInfo>>> searchUser([FromRoute] string input)
        {
            var user = _resumeRocketQueryUserBuilder.GetResumeRocketQueryUser(User);
            var result = await _chatService.searchUserAccount(user.AccountId, input);
            return _serviceResponseBuilder.BuildServiceResponse(result, HttpStatusCode.OK);

        }

        [HttpGet]
        [Route("UserInfo/{theyId}")]
        public async Task<ServiceResponseGeneric<AccountResponseBody>> Get([FromRoute] int theyId)
        {
            var accountResponse = await _accountService.GetAccountAsync(theyId);

            var response = new AccountResponseBody
            {
                FirstName = accountResponse.FirstName,
                LastName = accountResponse.LastName,
                ProfilePhotoLink = accountResponse.ProfilePhotoLink,
                Title = accountResponse.Title,
                Email = accountResponse.EmailAddress,
                Location = accountResponse.StateLocation,
                PortfolioLink = accountResponse.PortfolioLink,
                PrimaryResumeId = accountResponse.PrimaryResumeId,
                Skills = accountResponse.Skills,
                Experience = accountResponse.Experience.Select(e => new Domain.Api.Response.Experience
                {
                    Company = e.Company,
                    Description = e.Description,
                    EndDate = e.EndDate,
                    Position = e.Position,
                    StartDate = e.StartDate,
                    Type = e.Type
                }).ToList(),
                Education = accountResponse.Education.Select(edu => new Domain.Api.Response.Education
                {
                    Degree = edu.Degree,
                    GraduationDate = edu.GraduationDate,
                    Major = edu.Major,
                    Minor = edu.Minor,
                    SchoolName = edu.SchoolName
                }).ToList()
            };

            return _serviceResponseBuilder.BuildServiceResponse(response, HttpStatusCode.OK);
        }

    }
}
