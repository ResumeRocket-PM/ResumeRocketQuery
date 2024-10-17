namespace ResumeRocketQuery.Domain.Services
{
    public class ResumeResult
    {
        public int ResumeId { get; set; }
        public int AccountId { get; set; }
        public string? Resume { get; set; }
        public int? OriginalResumeID { get; set; }
        public bool OriginalResume { get; set; }
        public int Version { get; set; }
        public System.DateTime InsertDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string Html { get; set; }
    }
}
