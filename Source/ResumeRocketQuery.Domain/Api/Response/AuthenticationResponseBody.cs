namespace ResumeRocketQuery.Domain.Api.Response
{
    public class AuthenticationResponseBody
    {
        public bool IsAuthenticated { get; set; }
        public string JsonWebToken { get; set; }
    }
}
