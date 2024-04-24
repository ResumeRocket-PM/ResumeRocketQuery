using ResumeRocketQuery.Domain.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using NScrape;
using System.Text.RegularExpressions;

namespace ResumeRocketQuery.External
{
    
    public class jobScraper : IJobScraper 
    {
        public async Task<string> ScrapeJobPosting(string url)
        {
            try
            {
                var httpClient = new HttpClient();

                var html = await httpClient.GetStringAsync(url);

                var scraper = new TestScraper(html);

                var result = scraper.get_HTML_Body();

                return result;

            }
            catch (Exception e)
            {
                throw new Exception($"Error occurred while scraping: {e.Message}");
            }
        }
    }



    public class TestScraper : Scraper
    {
        public TestScraper(string html) : base(html)
        {
        }

        public string get_HTML_Body()
        {
            var node = HtmlDocument.DocumentNode.SelectSingleNode("//html");


            if (node != null)
            {
                var result = Regex.Replace(node.InnerText, @"[^a-zA-Z0-9 -]", string.Empty);
                
                result = Regex.Replace(result, @"\s\s+", " ");

                return result;
            }

            throw new ScrapeException("Could not scrape conditions.", Html);
        }
    }
}
