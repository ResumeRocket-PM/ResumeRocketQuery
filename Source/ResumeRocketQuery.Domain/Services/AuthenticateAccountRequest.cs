namespace ResumeRocketQuery.Domain.Services
{
    public class AuthenticateAccountRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
