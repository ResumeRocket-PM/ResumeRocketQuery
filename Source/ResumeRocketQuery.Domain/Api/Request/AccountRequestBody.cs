using System.ComponentModel.DataAnnotations;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class AccountRequestBody
    {
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
