using System.Collections.Generic;
using ResumeRocketQuery.Domain.External;

namespace ResumeRocketQuery.Domain.Services;

public class ApplicationResponse
{
    public string NewPDFHtml { get; set; }
    public List<Change> Changes { get; set; }
}