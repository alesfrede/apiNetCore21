using Microsoft.AspNetCore.Mvc;

namespace Api213.V2.Exception
{
    /// <summary>
    /// IInvalidResponseFactory
    /// </summary>
    public interface IInvalidResponseFactory
    {
        /// <summary>
        /// InvalidResponseFactory for ObjectResult
        /// </summary>
        /// <param name="objectResult"></param>
        /// <returns></returns>
        IActionResult Response(ObjectResult objectResult);

        /// <summary>
        /// InvalidResponseFactory for ObjectResult
        /// </summary>
        /// <param name="objectResult"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        IActionResult Response(ObjectResult objectResult, string shortHumanReadableSummary);

        /// <summary>
        /// Errors : InvalidResponseFactory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        IActionResult Response(int? code, string extraMessage, string shortHumanReadableSummary = null);

        /// <summary>
        /// Errors : InvalidResponseFactory
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extraMessage"></param>
        /// <param name="shortHumanReadableSummary"></param>
        /// <returns></returns>
        IActionResult ResponseGenericResult(int? code, string extraMessage, string shortHumanReadableSummary = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        void WriteAsync(System.Exception exception);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerBase"></param>
        void SetController(ControllerBase controllerBase);
    }
}