using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api213
{
    /// <inheritdoc cref="ActionFilterAttribute" />
    /// <summary>
    /// </summary>
    public class UnhandledExceptionFilterAttribute : ActionFilterAttribute, IExceptionFilter
    {
        /// <inheritdoc /> 
        /// <summary>
        /// Global OnException
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            context.Result = new BadRequestObjectResult(context.Exception.Message);
        }
    }
}
