using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public interface IJobScraper
    {

        Task<string> ScrapeJobPosting(string url);
    }
}
