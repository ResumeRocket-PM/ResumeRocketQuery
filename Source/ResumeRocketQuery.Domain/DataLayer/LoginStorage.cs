namespace ResumeRocketQuery.Domain.DataLayer
{
    public class LoginStorage
    {
        public int LoginId { get; set; }
        public int AccountId { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
    }
}
