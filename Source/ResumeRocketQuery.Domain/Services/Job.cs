using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Services;

public class Job
{
    public int AccountId { get; set; }
    public string JobUrl { get; set; }
    public Dictionary<string, string> Resume { get; set; }
}