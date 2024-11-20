namespace ResumeRocketQuery.Domain.Services;

public class ResumeSuggestions
{
    public int ResumeChangeId { get; set; }
    public string OriginalText { get; set; }
    public string ModifiedText { get; set; }
    public string ExplanationString { get; set; }
    public bool Accepted { get; set; }
    public string HtmlID { get; set; }
    public int ApplicationId { get; set; }
}