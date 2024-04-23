using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Api.Response;

public class JobPostingsResponse : List<JobPostingResponse>
{
    public JobPostingsResponse(IEnumerable<JobPostingResponse> postings) : base(postings)
    {
            
    }
}