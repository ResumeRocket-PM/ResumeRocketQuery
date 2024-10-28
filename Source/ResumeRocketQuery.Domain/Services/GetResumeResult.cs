using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Services;

public class GetResumeResult
{
    public int ResumeId { get; set; }
    public string ResumeHTML { get; set; }
    public List<ResumeSuggestions> ResumeSuggestions { get; set; }
}