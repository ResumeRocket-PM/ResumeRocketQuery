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
using OpenQA.Selenium.DevTools.V128.FileSystem;
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
        private string SetupDynamicContent(string waitedEle = "input")
        {
            // Set up Chrome options for headless mode
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless"); // Run in headless mode (no GUI)
            options.AddArgument("--disable-gpu"); // Applicable to Windows environments
            options.AddArgument("--window-size=1920,1080"); // Set a window size (needed in some cases)
            options.AddArgument("--no-sandbox"); 
            IWebDriver driver = new ChromeDriver(options);

            // Navigate to the target URL
            driver.Navigate().GoToUrl(this._url);

            // Set up a wait to ensure dynamic content is loaded
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Wait for the specific element to be present
            wait.Until(d => d.FindElement(By.TagName($"{waitedEle}")));

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
                string htmlpage = SetupDynamicContent("div");
                return htmlpage;

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
        public async Task<List<string>> TextInputFieldNames()
        {
            //TODO: this line is not useful, because this is a async method, there has to be an "await", but I haven't find a way to fix it
            var useless = await _httpClient.GetStringAsync(this._url);

            try
            {
                string htmlPage = SetupDynamicContent();

                var scraper = new TestScraper(htmlPage);

                return scraper.findInputField("text", "name");
            }
            catch (Exception e) 
            {

                throw new Exception($"{e}");
            }
        }

        public async Task<List<string>> CheckBoxInputFieldNames()
        {
            //TODO: this line is not useful, because this is a async method, there has to be an "await", but I haven't find a way to fix it
            var useless = await _httpClient.GetStringAsync(this._url);

            try
            {
                string htmlPage = SetupDynamicContent();

                var scraper = new TestScraper(htmlPage);

                return scraper.findInputField("checkbox","label");
            }
            catch (Exception e)
            {

                throw new Exception($"{e}");
            }
        }

        //public async Task<Dictionary<string, string>> FindInputValue(List<string> filedNames, int accountID)
        //{

        //}

        
        /// <summary>
        /// auto filled all the value into related value part of the input field of the html and submit that request
        /// 
        /// reference: "https://youtu.be/9zCJyLruWaE?si=O6_iC8q-wQjyJeiy"
        /// </summary>
        /// <param name="filedInputs"></param>
        /// <returns></returns>
        public async Task<bool> submitFilledForm(Dictionary<string, string> filedInputs)
        {

            try
            {
                IWebDriver testDriver = new ChromeDriver();
                testDriver.Navigate().GoToUrl(this._url);

                foreach (KeyValuePair<string, string> entry in filedInputs)
                {
                    IWebElement inputEle = testDriver.FindElement(By.Name(entry.Key));
                    //if (entry.Value == "cb")//cb: checkbox
                    //{
                    //    if (!inputEle.Selected)
                    //    {
                    //        inputEle.Click();
                    //    }
                    //}
                    if (inputEle != null)
                    {
                        inputEle.SendKeys(entry.Value);
                    }
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }

            //-------code below are useless--------
            var fillContent = new FormUrlEncodedContent(filedInputs);
            var response = await _httpClient.PostAsync(this._url, fillContent);
        }

        public async Task<bool> SaveHtmlFile(string fileName)
        {
            try
            {
                //IWebDriver driver = new ChromeDriver();

                //// Navigate to the target URL
                //driver.Navigate().GoToUrl(this._url);

                //// Set up a wait to ensure dynamic content is loaded
                //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                //// Wait for the specific element to be present
                //wait.Until(d => d.FindElement(By.TagName($"div")));

                //// Now that the page is fully loaded, get the page source
                //string htmlContent = driver.PageSource;

                string htmlContent = SetupDynamicContent("div");
                string htmlPage = await this._httpClient.GetStringAsync(this._url);
                System.IO.File.WriteAllText($"{fileName}.html", htmlContent);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"{e}");
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
        /// 
        /// parameter "inputType": it should be the type of input marked in HTML (e.g. text, dropdowns, check box, radio button)
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        /// <exception cref="ScrapeException"></exception>
        public List<string> findInputField(string inputType, string attributeName)
        {
            List<string> fieldNames = new List<string>();

            //HtmlDocument.LoadHtml(this.Html);
            var inputFields = HtmlDocument.DocumentNode.SelectNodes($"//input[@type='{inputType}']");

            if (inputFields != null)
            {
                foreach (var inputField in inputFields)
                {
                    string fieldName = inputField.GetAttributeValue($"{attributeName}", "");
                    //string fieldId = inputField.GetAttributeValue("id","");
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        fieldNames.Add(fieldName);
                    }
                }
                return fieldNames;
            }
            throw new ScrapeException($"Could not scrape conditions with the type {inputType}, please make sure the type is correct.", Html);

        }

    }
}


