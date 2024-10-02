using Core;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SkyLearn.Portal.Api.Helpers
{
    public class JsonResponse
    {
        public static JsonResult CreateJsonResponse(HttpStatusCode? statusCode = HttpStatusCode.Unauthorized, string? message = "", object? data = null)
        {
            var jsonResult = new JsonResult(ObjectResponse.CreateResponse(statusCode, message, data));
            jsonResult.StatusCode = (int)statusCode;
            return jsonResult;
        }
    }
}
