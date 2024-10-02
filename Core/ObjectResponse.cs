using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ObjectResponse
    {
        public static object CreateResponse(HttpStatusCode? statusCode = HttpStatusCode.Unauthorized, string? message = "", object? data = null)
        {
            ApiResponse apiResponse = new ApiResponse
            {
                data = data,
                status = (HttpStatusCode)statusCode,
                message = message,
                success = ((int)statusCode >= 200 && (int)statusCode < 300) ? true : false,
            };
            if (!apiResponse.success)
            {
                apiResponse.error_code =(int)statusCode;
            }
            return apiResponse;
        }
    }
}
