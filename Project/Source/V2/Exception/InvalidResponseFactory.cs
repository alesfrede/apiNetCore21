using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Newtonsoft.Json;

namespace Api213.V2.Exception
{
    /// <inheritdoc cref="IInvalidResponseFactory" />
    /// <summary>
    /// InvalidResponseFactory
    /// </summary>
    public sealed class InvalidResponseFactory : IInvalidResponseFactory, IErrorResponseProvider
    {
        /// <summary>
        /// 
        /// </summary>
        private const int GenericError = 500;

        /// <summary>
        /// 
        /// </summary>
        private ControllerBase _controller;

        /// <summary>
        /// 
        /// </summary>
        private HttpContext _httpContext;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseFactory"/> class.
        /// 
        /// </summary>
        /// <param name="petsController"></param>
        public InvalidResponseFactory(ControllerBase petsController)
        {
            _controller = petsController;
            _httpContext = _controller.HttpContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseFactory"/> class.
        /// 
        /// </summary>
        public InvalidResponseFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseFactory"/> class.
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        public InvalidResponseFactory(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        /// <inheritdoc />
        /// <summary>
        /// InvalidResponseFactory for ObjectResult
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public IActionResult Response(ObjectResult objectResult)
        {
            _httpContext = _controller.HttpContext;
            return Response(objectResult.StatusCode, objectResult.Value.ToString());
        }

        /// <inheritdoc />
        /// <summary>
        /// InvalidResponseFactory for ObjectResult
        /// </summary>
        /// <param name="objectResult"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        public IActionResult Response(ObjectResult objectResult, string shortHumanReadableSummary)
        {
            _httpContext = _controller.HttpContext;
            return Response(objectResult.StatusCode, objectResult.Value.ToString(), shortHumanReadableSummary);
        }

        /// <inheritdoc />
        /// <summary>
        /// Errors : InvalidResponseFactory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        public IActionResult Response(int? code, string extraMessage, string shortHumanReadableSummary = null)
        {
            _controller.ModelState.AddModelError("ExtraMessage", extraMessage);

            return MakeObjectResult(code, extraMessage, shortHumanReadableSummary);
        }

        /// <inheritdoc />
        /// <summary>
        /// Errors : InvalidResponseFactory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        public IActionResult ResponseGenericResult(int? code, string extraMessage, string shortHumanReadableSummary = null)
        {
            return MakeObjectResult(code, extraMessage, shortHumanReadableSummary);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="exception"></param>
        public void WriteAsync(System.Exception exception)
        {
            var response = _httpContext.Response;
            response.Clear();
            response.ContentType = "application/problem+json";
            response.StatusCode = _httpContext.Response.StatusCode;
            
            response.WriteAsync(JsonConvert.SerializeObject(
                ResponseGenericResult(
                    _httpContext.Response.StatusCode,
                "extramesa",
                "Exception")));
        }

        /// <inheritdoc />
        /// <summary>
        /// SetController
        /// </summary>
        /// <param name="controllerBase"></param>
        public void SetController(ControllerBase controllerBase)
        {
            _controller = controllerBase;
            _httpContext = _controller.HttpContext;
        }

        /// <inheritdoc />
        /// <summary>
        /// IErrorResponseProvider
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IActionResult CreateResponse(ErrorResponseContext context)
        {
            _httpContext = context.Request.HttpContext;
            return MakeObjectResult(context.StatusCode, context.Message, context.ErrorCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        private IActionResult MakeObjectResult(int? code, string extraMessage, string shortHumanReadableSummary)
        {
            var codehttp = GenericError;
            if (code != null && code <= 511 && code >= 100)
                codehttp = (int)code;
            var problemDetails = _controller != null ? new ErrorDetails(_controller.ModelState) : new ErrorDetails();
            problemDetails.Instance = _httpContext.Request.Path;
            problemDetails.Status = codehttp;
            problemDetails.Type = "https://asp.net/core";
            problemDetails.Detail = "ExtraMessage: " + extraMessage + (char)13 +
                                    ", Please refer to the errors property for additional details.";
            problemDetails.HttpMethod = _httpContext.Request.Method;

            if (shortHumanReadableSummary != null)
                problemDetails.Title = shortHumanReadableSummary;

            var type = new MediaTypeCollection { "application/problem+json" };
            var objectResult = new ObjectResult(problemDetails)
            {
                ContentTypes = type,
                StatusCode = codehttp
            };
            return objectResult;
        }
    }
}