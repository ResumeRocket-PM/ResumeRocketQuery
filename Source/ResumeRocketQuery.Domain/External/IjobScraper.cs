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
        Task<string> ScrapeJobPosting(string target);
        Task<List<string>> TextInputFieldNames();
        Task<List<string>> CheckBoxInputFieldNames();
        Task<bool> submitFilledForm(Dictionary<string, string> filedInputs);
        Task<bool> SaveHtmlFile(string fileName);

    }
}
