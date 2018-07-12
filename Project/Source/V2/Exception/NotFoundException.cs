namespace Api213.V2.Exception
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class NotFoundException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// 
        /// </summary>
        public NotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NotFoundException(string message) 
            : base(message)
        {
        }
    }
}
