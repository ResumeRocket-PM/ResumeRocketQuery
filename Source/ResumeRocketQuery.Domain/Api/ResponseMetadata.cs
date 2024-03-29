using System.Collections.Generic;

namespace ResumeRocketQuery.Domain.Api
{
    public class ResponseMetadata
    {
        public int HttpStatusCode { get; set; }
        public string Exception { get; set; }
        public List<ValidationError> ValidationErrors { get; set; }
    }
}
