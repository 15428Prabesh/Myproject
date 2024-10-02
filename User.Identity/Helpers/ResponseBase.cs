using Microsoft.AspNetCore.Http;
using System.Net;

namespace User.Identity.Helpers
{
    public abstract class ResponseBase
    {

        /// <summary>
        /// Defines the status of respose 
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        /// <summary>
        /// Clear message that defines the respose clearly
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Holds the result object to be transmitted
        /// </summary>
        public object Result { get; set; }
    }
}
