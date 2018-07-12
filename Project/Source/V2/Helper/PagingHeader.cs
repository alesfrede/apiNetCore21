using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api213.V2.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class PagingHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagingHeader"/> class.
        /// PagingHeader
        /// </summary>
        /// <param name="totalItems"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalPages"></param>
        public PagingHeader(
            int totalItems, int pageNumber, int pageSize, int totalPages)
        {
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
        }

        /// <summary>
        /// 
        /// </summary>
        public int TotalItems { get; }

        /// <summary>
        /// 
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(
                this,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
        }
    }
}
