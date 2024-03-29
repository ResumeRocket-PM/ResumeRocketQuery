namespace ResumeRocketQuery.Domain.Services
{
    public class PasswordHashRequest
    {
        public string Password { get; set; }
        public string Salt { get; set; }

    }
}
