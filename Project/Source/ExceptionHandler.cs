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

                if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    if (!context.Response.HasStarted)
                    {
                        new InvalidResponseFactory(context).WriteAsync(null);
                    }
                    else
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = 500;
                        new InvalidResponseFactory(context).WriteAsync(null);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                {
                    new InvalidResponseFactory(context).WriteAsync(ex);
                }
                else
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 500;
                    new InvalidResponseFactory(context).WriteAsync(ex);
                }
            }
        }
    }
}