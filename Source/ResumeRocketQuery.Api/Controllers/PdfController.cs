using System.Net;
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

        /*/// <summary>
        ///     This retrieves the PDF content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpPost]
        [Route("read")]
        public async Task<ServiceResponse<string>> Get([FromBody] string request) // TODO
        {
            var pdfResult = await pdfService.ReadPdfAsync(""); // TODO

            // TODO

            return _serviceResponseBuilder.BuildServiceResponse(pdfResult, HttpStatusCode.OK); // TODO
        }

        /// <summary>
        ///     This retrieves the PDF content
        /// </summary>
        /// <returns>A PDF Object</returns>
        [HttpPost]
        [Route("update")]
        public async Task<ServiceResponse<string>> Post([FromBody] string request) // TODO
        {
            var pdfResult = await pdfService.UpdatePdfAsync("", ""); // TODO

            // TODO

            return _serviceResponseBuilder.BuildServiceResponse(pdfResult, HttpStatusCode.OK); // TODO
        }*/
    }
}
