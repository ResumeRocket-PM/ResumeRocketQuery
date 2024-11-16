using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class ResumeSuggestionsUpdateRequest
    {
        public bool Accepted { get; set; }
    }
}
