using System.Collections.Generic;
using Api213.V2.Exception;

namespace ApiXUnitTestProject
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class ErrorDetailsTests : IErrorDetails
    {
        public ErrorDetailsTests()
        {
            Errors = new Dictionary<string, string[]>();
        }

        public string HttpMethod { get; set; }

        public IDictionary<string, string[]> Errors { get; }

        public string Type { get; set; }

        public string Title { get; set; }

        public int? Status { get; set; }

        public string Detail { get; set; }

        public string Instance { get; set; }
    }
}
