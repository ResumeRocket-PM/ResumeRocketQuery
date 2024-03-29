using System.ComponentModel.DataAnnotations;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class AuthenticationRequestBody
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
