namespace Api213.V2.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class FilteringSortingParams 
    {
        /// <summary>
        /// Gets or sets sort use: +property,-property
        /// </summary>
        public string Sort { get; set; } = string.Empty;

        /// <summary>
        /// Filter: propertyname,propertyname,propertyname
        /// </summary>
        public string Fields { get; set; } = string.Empty;

        /// <summary>
        /// PageNumber
        /// </summary>
        public int Page
        {
            get => PagingParams.PageNumber;
            set => PagingParams.PageNumber = value;
        }

        /// <summary>
        /// PageSize
        /// </summary>
        public int Count
        {
            get => PagingParams.PageSize;
            set => PagingParams.PageSize = value;
        }

        /// <summary>
        /// PagingParams
        /// </summary>
        private PagingParams PagingParams { get;  } = new PagingParams();
    }
}
