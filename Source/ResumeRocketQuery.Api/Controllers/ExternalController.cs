using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Domain.External;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Api.Request;
using Sprache;
using ResumeRocketQuery.Api.Builder;
using System.Net;
using NScrape;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using iText.Layout.Element;
using System.Collections.Generic;
//using OpenQA.Selenium.DevTools.V126.Autofill;
using System.IO;

namespace ResumeRocketQuery.Api.Controllers
{
    //[Authorize]
    [Route("api/Externel")]
    public class ExternalController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IJobScraper _scraper;
        private readonly IOpenAiClient _openAi;
        public ExternalController(IJobScraper jobscraper, IOpenAiClient openAiClient, IServiceResponseBuilder serviceResponseBuilder) 
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _scraper = jobscraper;
            _openAi = openAiClient;
        }

        
        // TODO: now sure if need provide the route
        /// <summary>
        /// Use scrape method to scrape the expected content of web page given by the url and the token domain
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("scraper")]
        public async Task<ServiceResponseGeneric<string>> getScraper(string token, string url)
        {
            try
            {
                this._scraper.ScrapeSetup(url);
                var scraperResult = await this._scraper.ScrapeJobPosting(token);
                return _serviceResponseBuilder.BuildServiceResponse(scraperResult, HttpStatusCode.OK);
            }
            catch (Exception e)
            {

                throw new Exception($"Ops! something goes wrong please make sure your url is correct\n Error: {e}");
            }
        }

        /// <summary>
        /// provide the url that should be a job application filling page, and the using the IjobScraper to auto fill the info
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("autoFill")]
        public async Task<ServiceResponse> AutoFillForm(string url)
        {
            this._scraper.ScrapeSetup(url);
            var FileNameList = await this._scraper.TextInputFieldNames();
            for (int i = 0;i<FileNameList.Count; i++)
            {

            }
            Dictionary<string, string> test = new Dictionary<string, string>();

            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("openai/aiMessage")]
        public async Task<ServiceResponseGeneric<string>> AiSendMessageAsync(string prompt, string message)
        {
            string responseMsg = await this._openAi.SendMessageAsync(prompt, message);
            return _serviceResponseBuilder.BuildServiceResponse(responseMsg, HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("openai/aiMultipleMessage")]
        public async Task<ServiceResponseGeneric<string>> AiSendMultiMessageAsync([FromBody]SendAiMessage sendAiMessage)
        {
            string responseMsg = await _openAi.SendMultiMessageAsync(sendAiMessage.resumeId, sendAiMessage.applicationId, sendAiMessage.message);
            return _serviceResponseBuilder.BuildServiceResponse(responseMsg, HttpStatusCode.OK);
        }
    }
}
