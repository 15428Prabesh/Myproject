using System.Net;
using System.Text.Json.Serialization;

namespace Core
{
    public class ApiResponse : BaseResponse
    {
    }

    public class LoginResonse
    {
        public HttpStatusCode status { get; set; } = HttpStatusCode.OK;
        public string message { get; set; } = string.Empty;
        public Data data { get; set; }
        public bool success { get; set; }
        public int error_code { get; set; }
    }

    public class Data
    {
        public string Idx { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public string JwtToken { get; set; }

    }

    public class UserDetailResponse
    {
         /// <summary>
        /// Defines the status of respose 
        /// </summary>
        public HttpStatusCode status { get; set; } = HttpStatusCode.OK;
        /// <summary>
        /// Clear message that defines the respose clearly
        /// </summary>
        public string message { get; set; }=string.Empty;
        /// <summary>
        /// Holds the result object to be transmitted
        /// </summary>
        public UserDetailData data { get; set; }
        public bool success { get; set; }
        public int error_code { get; set; }

    }

    public class UserDetailData
    {
        public string Idx { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public object RolesPid{get;set;}
    }

}
