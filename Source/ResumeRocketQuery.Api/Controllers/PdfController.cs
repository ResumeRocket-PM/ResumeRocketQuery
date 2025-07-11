﻿using System.Net;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Services;
using System.Collections.Generic;

namespace ResumeRocketQuery.Api.Controllers
{
    /// <summary>
    ///     This is a controller that handles the retrieval and modification of PDFs.
    /// </summary>
    [Authorize]
    [Route("api/pdf")]
    public class PdfController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IPdfService pdfService;

        public PdfController(IServiceResponseBuilder serviceResponseBuilder, IPdfService pdfService)
        {
            this._serviceResponseBuilder = serviceResponseBuilder;
            this.pdfService = pdfService;
        }

        /// <summary>
        ///     This retrieves the PDF content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpGet]
        [Route("read")]
        public async Task<ServiceResponseGeneric<string>> Get(string request)
        {
            var pdfResult = await pdfService.ReadPdfAsync(request);
            return _serviceResponseBuilder.BuildServiceResponse(pdfResult, HttpStatusCode.OK);
        }

        /// <summary>
        ///     This retrieves the PDF content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpPost]
        [Route("update")]
        public async Task<ServiceResponse> Post([FromBody] string request, string update)
        {
            var pdfResult = await pdfService.UpdatePdfAsync(request, update); 
            return _serviceResponseBuilder.BuildServiceResponse(HttpStatusCode.OK);
        }
    }
}
