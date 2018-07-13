using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api213.V2.Exception
{
    /// <inheritdoc />
    /// <summary>
    /// A machine-readable format for specifying errors in HTTP API responses based on
    ///     https://tools.ietf.org/html/rfc7807.
    /// 
    /// </summary>
    public class ErrorDetails : ValidationProblemDetails, IErrorDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDetails"/> class.
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        public ErrorDetails(ModelStateDictionary modelState)
            : base(modelState)
        {
        }

        /// <summary>
        /// HttpStatusCode
        /// </summary>
        public int HttpStatusCode { get; set; }

        /// <summary>
        /// HttpMethod
        /// </summary>
        public string HttpMethod { get; set; }
    }
}
