using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class AccountDetailsRequest
    {
        public Dictionary<string, string> Parameters { get; set; }
    }
}
