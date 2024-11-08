using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services.Repository
{
    public class FriendInfo
    {
        public int FriendsId{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhotoLink { get; set; }
        public string PortfolioLink { get; set;}
        public string EmailAddress { get; set; }
        public string CreatedTime { get; set; }

    }
}
