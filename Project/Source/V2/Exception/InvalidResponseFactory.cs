using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace Api213.V2.Exception
{
    /// <summary>
    /// InvalidResponseFactory
    /// </summary>
    public class InvalidResponseFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private const int GenericError = 500;

        /// <summary>
        /// 
        /// </summary>
        private readonly ControllerBase _controller;

        /// <summary>
        /// 
        /// </summary>
        private HttpContext _context;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseFactory"/> class.
        /// 
        /// </summary>
        /// <param name="petsController"></param>
        public InvalidResponseFactory(ControllerBase petsController)
        {
            _controller = petsController;
            _context = _controller.HttpContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseFactory"/> class.
        /// 
        /// </summary>
        /// <param name="context"></param>
        public InvalidResponseFactory(HttpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// InvalidResponseFactory for ObjectResult
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public IActionResult Response(ObjectResult objectResult)
        {
            _context = _controller.HttpContext;
            return Response(objectResult.StatusCode, objectResult.Value.ToString());
        }

        /// <summary>
        /// InvalidResponseFactory for ObjectResult
        /// </summary>
        /// <param name="objectResult"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        public IActionResult Response(ObjectResult objectResult, string shortHumanReadableSummary)
        {
            _context = _controller.HttpContext;
            return Response(objectResult.StatusCode, objectResult.Value.ToString(), shortHumanReadableSummary);
        }

        /// <summary>
        /// Errors : InvalidResponseFactory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        public IActionResult Response(int? code, string extraMessage, string shortHumanReadableSummary = null)
        {
            var codehttp = GenericError;
            if (code != null && code <= 511 && code >= 100)
                codehttp = (int)code;

            _controller.ModelState.AddModelError("ExtraMessage", extraMessage);
            var problemDetails = new ErrorDetails(_controller.ModelState)
            {
                Instance = _context.Request.Path,
                Status = codehttp,
                Type = "https://asp.net/core",
                Detail = "ExtraMessage: " + extraMessage + (char)13 +
                         ", Please refer to the errors property for additional details.",
                HttpMethod = _context.Request.Method
             };
            if (shortHumanReadableSummary != null)
                problemDetails.Title = shortHumanReadableSummary;

            var type = new MediaTypeCollection { "application/problem+json" };
            var objectResult = _controller.StatusCode(codehttp, problemDetails);
            objectResult.ContentTypes = type;
            return objectResult;
        }

        /// <summary>
        /// Errors : InvalidResponseFactory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        public IActionResult Response2(int? code, string extraMessage, string shortHumanReadableSummary = null)
        {
            var codehttp = GenericError;
            if (code != null && code <= 511 && code >= 100)
                codehttp = (int)code;

            var problemDetails = new ErrorDetails
            {
                Instance = _context.Request.Path,
                Status = codehttp,
                Type = "https://asp.net/core",
                Detail = "ExtraMessage: " + extraMessage + (char)13 +
                         ", Please refer to the errors property for additional details.",
                HttpMethod = _context.Request.Method
            };
            if (shortHumanReadableSummary != null)
                problemDetails.Title = shortHumanReadableSummary;

            var type = new MediaTypeCollection { "application/problem+json" };
            var objectResult = new ObjectResult(problemDetails) {ContentTypes = type};
            return objectResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void WriteAsync(System.Exception exception)
        {
            var response = _context.Response;
            response.Clear();
            response.ContentType = "application/problem+json";
            response.StatusCode = _context.Response.StatusCode;
            
            response.WriteAsync(JsonConvert.SerializeObject(
                Response2(
                    _context.Response.StatusCode,
                "extramesa",
                "Exception")));
        }
    }
}