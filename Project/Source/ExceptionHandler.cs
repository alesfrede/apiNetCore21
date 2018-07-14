using System;
using System.Threading.Tasks;
using Api213.V2.Exception;
using Microsoft.AspNetCore.Http;

namespace Api213
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        /// <inheritdoc />
        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
               
                if (context.Response.StatusCode != StatusCodes.Status200OK &&
                    context.Response.StatusCode != StatusCodes.Status201Created)
                {
                    new InvalidResponseFactory(context).WriteAsync(null);
                }
            }
            catch (Exception ex)
            {
                new InvalidResponseFactory(context).WriteAsync(ex);
            }
        }
    }
}