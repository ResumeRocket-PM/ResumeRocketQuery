using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Api.Response
{
    public class CreateJobPostingResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> Perks { get; set; }
        public List<string> Requirements { get; set; }
    }
}
