using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Api213.V2.Exception
{
    /// <summary>
    /// InvalidResponseFactory
    /// </summary>
    public class InvalidResponseFactory
    {
        private const int GenericError = 500;

        private readonly ControllerBase _controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseFactory"/> class.
        /// 
        /// </summary>
        /// <param name="petsController"></param>
        public InvalidResponseFactory(ControllerBase petsController)
        {
            _controller = petsController;
        }

        /// <summary>
        /// InvalidResponseFactory for ObjectResult
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        public IActionResult Response(ObjectResult objectResult)
        {
            return Response(objectResult.StatusCode, objectResult.Value.ToString());
        }

        /// <summary>
        /// Errors : InvalidResponseFactory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <returns></returns>
        public IActionResult Response(int? code, string extraMessage)
        {
            var codehttp = GenericError;
            if (code != null && code < 512 && code >= 100)
                codehttp = (int)code;

            _controller.ModelState.AddModelError("ExtraMessage", extraMessage);
            var problemDetails = new ErrorDetails(_controller.ModelState)
            {
                Instance = _controller.HttpContext.Request.Path,
                Status = code,
                Type = "https://asp.net/core",
                Detail = "ExtraMessage: " + extraMessage + (char)13 +
                         ", Please refer to the errors property for additional details.",
                HttpMethod = _controller.HttpContext.Request.Method,
                HttpStatusCode = codehttp
            };

            var type = new MediaTypeCollection { "application/problem+json" };
            var objectResult = _controller.StatusCode(codehttp, problemDetails);
            objectResult.ContentTypes = type;
            return objectResult;
        }
    }
}