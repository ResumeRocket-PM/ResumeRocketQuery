namespace ResumeRocketQuery.Domain.Api.Request
{
    public class SearchAccountRequest
    {
        public string SearchTerm { get; set; }
        public int ResultCount { get; set; }
    }
}
