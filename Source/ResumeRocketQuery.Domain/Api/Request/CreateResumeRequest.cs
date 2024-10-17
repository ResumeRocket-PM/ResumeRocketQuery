using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Api.Request
{
    public class CreateResumeRequest
    {
        public bool? OriginalResume { get; set; }
        public string? ResumeHtmlString { get; set; }
    }
}
