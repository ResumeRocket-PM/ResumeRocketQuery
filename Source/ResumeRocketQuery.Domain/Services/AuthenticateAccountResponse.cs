namespace ResumeRocketQuery.Domain.Services
{
    public class AuthenticateAccountResponse
    {
        public bool IsAuthenticated { get; set; }
        public string JsonWebToken { get; set; }
    }
}
