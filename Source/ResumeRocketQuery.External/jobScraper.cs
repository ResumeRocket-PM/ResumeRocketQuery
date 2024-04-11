using ResumeRocketQuery.Domain.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using NScrape;

namespace ResumeRocketQuery.External
{
    
    public class jobScraper : IjobScraper 
    {
        //private string Url;
        public class scraper() 
        {
        }

        public async Task<string> scrapJobPosting(string url)
        {
            try
            {
                //string temp_url = "https://www.1800contacts.com/job-listing?gh_jid=5849626";
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);//.Result;

                var scraper = new TestScraper(html);
                var result = scraper.get_HTML_Body();


                Console.WriteLine(html.ToString());

                return html;

            }
            catch (Exception e)
            {
                //Console.WriteLine($"Error occurred while scraping: {e.Message}");
                //return null;
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
            //var node = HtmlDocument.DocumentNode.Element("//body");//.Descendants().SingleOrDefault(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "myforecast-current");
            var node = HtmlDocument.DocumentNode.SelectSingleNode("//html");
            if (node != null)
            {
                return node.InnerText;
            }

            throw new ScrapeException("Could not scrape conditions.", Html);
        }
    }
}
