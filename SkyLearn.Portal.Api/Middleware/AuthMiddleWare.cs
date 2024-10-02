using Application.Models;
using Azure.Core;
using Core;
using Core.Constants;
using Core.Helper.APiCall;
using Microsoft.Extensions.Options;
using SkyLearn.Portal.Api.Services;
using System.Net;

namespace SkyLearn.Portal.Api.Middleware
{
    public class AuthMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly BaseUrl _baseUrl;
        public AuthMiddleWare(RequestDelegate next,IOptions<BaseUrl> baseUrl)
        {
            _next = next;
            _baseUrl = baseUrl.Value;
        }

        public async Task InvokeAsync(HttpContext context,UserRoleService userRoleService)
        {
            var token = context.Request.Headers["Authorization"];
            string origin = context.Request.Headers.Origin.ToString();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Origin", origin);
            headers.Add("Authorization", token);
            ApiClient? apiClient = new ApiClient();
            LoginResonse? apiResponse = await apiClient.GetAsyncResult<LoginResonse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.ValidateToken, HttpMethod.Get, headers);
            

            if (apiResponse!=null && apiResponse.status == HttpStatusCode.OK)
            {
                var data = apiResponse.data;
                var user_role_obj =await userRoleService.GetUserRoles(data.Idx);
                var userInRoles=await userRoleService.GetRoles(data.Idx);
                List<string> roleList = userInRoles.Select(x => x.Name).ToList();
                context.Items["user"] = data;
                context.Items["roles"] = user_role_obj;
                context.Items["userinroles"] = roleList;
                context.Items["origin"] = origin;
            }
            await _next(context);
        }
    }

    public static class AuthMiddleWareExtension
    {
        public static IApplicationBuilder UseMyCustomMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleWare>();
        }
    }
}
