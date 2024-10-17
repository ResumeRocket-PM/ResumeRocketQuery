using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Services;

public class ApplicationRequest
{
    public int AccountId { get; set; }
    public string JobUrl { get; set; }
    public string JobHtml { get; set; }
}