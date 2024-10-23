using System;

namespace ResumeRocketQuery.Domain.Api.Response
{
    public class ResumesResponseBody
    {
        public int ResumeId { get; set; }
        public int? OriginalResumeID { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
