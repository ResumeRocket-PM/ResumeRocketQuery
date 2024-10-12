namespace ResumeRocketQuery.Domain.Services
{
    public class ResumeRepository
    {
        public int ResumeId { get; set; }
        public int AccountId { get; set; }
        public string Resume { get; set; }
        public System.DateTime InsertDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
