namespace ResumeRocketQuery.Domain.Services
{
    public class CreateAccountResponse
    {
        public int AccountId { get; set; }
        public string JsonWebToken { get; set; }
    }
}
