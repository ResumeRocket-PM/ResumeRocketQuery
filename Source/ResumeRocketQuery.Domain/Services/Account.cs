namespace ResumeRocketQuery.Domain.Services
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountAlias { get; set; }
        public string EmailAddress { get; set; }
        public Authentication Authentication { get; set; }

    }
}
