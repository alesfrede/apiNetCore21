using System.Collections.Generic;

namespace Api213.V2.Exception
{
    /// <summary>
    /// 
    /// </summary>
    public interface IErrorDetails
    {
        /// <summary>
        /// HttpStatusCode
        /// </summary>
        int HttpStatusCode { get; set; }

        /// <summary>
        /// HttpMethod
        /// </summary>
        string HttpMethod { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        IDictionary<string, string[]> Errors { get; }

        /// <summary>
        /// 
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Detail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Instance { get; set; }
    }
}