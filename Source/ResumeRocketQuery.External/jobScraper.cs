using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.External
{
    public interface IjobScraper
    {

        Task<string> scrapJobPosting(string url);
    }
    public class jobScraper : IjobScraper 
    {
        public async Task<string> scrapJobPosting(string url)
        {
            throw new NotImplementedException();
        }

    }
}
