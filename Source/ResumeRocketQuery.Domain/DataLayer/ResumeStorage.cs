namespace ResumeRocketQuery.Domain.DataLayer
{
    public class ResumeStorage
    {
        public int ResumeId { get; set; }
        public int AccountId { get; set; }
        public string Resume { get; set; }

        public int? OriginalResumeID { get; set; }
        public bool OriginalResume { get; set; }
        public int Version { get; set; }
    }
}
