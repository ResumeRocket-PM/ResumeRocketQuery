namespace ResumeRocketQuery.Domain.Services
{
    public class CreateAccountRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
