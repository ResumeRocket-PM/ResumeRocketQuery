using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class ResumeSuggestionsUpdateRequest
    {
        public List<SuggestionStatus> SuggestionStatuses { get; set; }
    }

    public class SuggestionStatus
    {
        public int ResumeChangeId { get; set; }
        public bool IsApplied { get; set; }
    }
}
