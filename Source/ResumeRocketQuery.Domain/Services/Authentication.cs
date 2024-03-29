namespace ResumeRocketQuery.Domain.Services
{
    public class Authentication
    {
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }
}
