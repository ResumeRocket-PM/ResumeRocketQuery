namespace ResumeRocketQuery.Domain.Api.Request
{
    public class SendAiMessage
    {
        public int resumeId { get; set; }
        public int? applicationId { get; set; }
        public string message { get; set; } 
    }
}