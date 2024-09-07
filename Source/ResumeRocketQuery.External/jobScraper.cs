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
using System.ComponentModel;
using System.Collections.Specialized;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.IO;
//using Microsoft.Extensions.Logging;

namespace ResumeRocketQuery.External
{

    public class jobScraper : IJobScraper
    {
        private HttpClient _httpClient;
        private string _url;
        public void ScrapeSetup(string url)
        {
            this._httpClient = new HttpClient();
            this._url = url;
        }

        /// <summary>
        /// helper method for setup the dynamic web page content 
        /// to make sure that all html content is loaded completely
        /// </summary>
        /// <returns></returns>
        private string SetupDynamicContent()
        {
            // Set up Chrome options for headless mode
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless"); // Run in headless mode (no GUI)
            options.AddArgument("--disable-gpu"); // Applicable to Windows environments
            options.AddArgument("--window-size=1920,1080"); // Set a window size (needed in some cases)
            IWebDriver driver = new ChromeDriver(options);

            // Navigate to the target URL
            driver.Navigate().GoToUrl(this._url);
            // Set up a wait to ensure dynamic content is loaded
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Wait for the specific element to be present
            wait.Until(d => d.FindElement(By.TagName("input")));

            // Now that the page is fully loaded, get the page source
            string pageSource = driver.PageSource;
            driver.Quit();

            return pageSource;
        }
        /// <summary>
        /// provide the target of html, and scrap the html content inside the target field
        /// return the scraped result
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> ScrapeJobPosting(string target)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(this._url);

                var scraper = new TestScraper(html);

                scraper = new TestScraper(html);

                var result = scraper.get_HTML_Body(target);

                return result;

            }
            catch (Exception e)
            {
                throw new Exception($"Error occurred while scraping: {e.Message}");
            }
        }

        /// <summary>
        /// provide all the input field from the given html, and put those input field name into the list
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> InputFieldNames()
        {
            //TODO: this line is not useful, because this is a async method, there has to be an "await", but I haven't find a way to fix it
            var useless = await _httpClient.GetStringAsync(this._url);

            string htmlPage = SetupDynamicContent();

            var scraper = new TestScraper(htmlPage);

            return scraper.findInputField();
        }

        /// <summary>
        /// auto filled all the value into related value part of the input field of the html and submit that request
        /// </summary>
        /// <param name="filedInputs"></param>
        /// <returns></returns>
        public async Task<string> submitFilledForm(Dictionary<string, string> filedInputs)
        {
            var fillContent = new FormUrlEncodedContent(filedInputs);

            var response = await _httpClient.PostAsync(this._url, fillContent);


            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Save the response HTML to a file
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "result.html");
                File.WriteAllText(filePath, responseBody);

                // Initialize WebDriver (make sure ChromeDriver is in your PATH)
                IWebDriver driver = new ChromeDriver();

                // Navigate to an about:blank page
                driver.Navigate().GoToUrl("about:blank");

                // Inject the HTML into the page using JavaScript
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.write(arguments[0]);", responseBody);

                // Optional: Maximize the window
                driver.Manage().Window.Maximize();

                return responseBody;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }


    }



    public class TestScraper : Scraper
    {
        public TestScraper(string html) : base(html)
        {
        }

        public string get_HTML_Body(string target)
        {
            // e.g.target == "//html"
            var node = HtmlDocument.DocumentNode.SelectSingleNode(target);


            if (node != null)
            {
                var result = Regex.Replace(node.InnerText, @"[^a-zA-Z0-9 -]", string.Empty);

                result = Regex.Replace(result, @"\s\s+", " ");

                return result;
            }

            throw new ScrapeException("Could not scrape conditions.", Html);
        }

        /// <summary>
        /// find all field from the scraped html and put the filed names into the list
        /// </summary>
        /// <returns></returns>
        public List<string> findInputField()
        {
            List<string> fieldNames = new List<string>();

            //HtmlDocument.LoadHtml(this.Html);
            var inputFields = HtmlDocument.DocumentNode.SelectNodes("//input[@name]");

            if (inputFields != null)
            {
                foreach (var inputField in inputFields)
                {
                    string fieldName = inputField.GetAttributeValue("name", "");
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        fieldNames.Add(fieldName);
                    }
                }
                return fieldNames;
            }
            throw new ScrapeException("Could not scrape conditions.", Html);

        }

    }
}


