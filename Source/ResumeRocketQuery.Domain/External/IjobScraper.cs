using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ResumeRocketQuery.Domain.External
{
    public interface IJobScraper
    {

        void ScrapeSetup(string url);
        Task<string> ScrapeJobPosting(string url);
        Task<List<string>> InputFieldNames();
        Task<string> submitFilledForm(Dictionary<string, string> filedInputs);

    }
}
