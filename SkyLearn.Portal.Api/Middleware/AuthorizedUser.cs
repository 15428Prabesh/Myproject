using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Reflection;
using Application.Models;
using Core;
using SkyLearn.Portal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace SkyLearn.Portal.Api.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizedUser : Attribute, IAsyncAuthorizationFilter
    {
        private readonly PageActionService pageActionService;
        private readonly PermissionService permissionService;

        public AuthorizedUser(PageActionService pageActionService, PermissionService permissionService)
        {
            this.pageActionService = pageActionService;
            this.permissionService = permissionService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Skip authorization if the action is decorated with [AllowAnonymous] attribute
            var controllerType = (context.ActionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo.AsType();
            var allowAnonymousController = IsControllerAllowAnonymous(controllerType);
            var allowAnonymousAction = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute));
            if (allowAnonymousController || allowAnonymousAction)
            {
                return; // Skip authorization
            }
            var account = (Data?)context.HttpContext.Items["user"];
            if (account == null)
            {
                throw new ForbiddenException("You do not have permission to access this resource.");
            }
            var roles = (List<UserRole>?)context.HttpContext.Items["roles"];

            var controllerAction = context.ActionDescriptor as ControllerActionDescriptor;
            var areaName = string.Empty;

            if (controllerAction.ControllerTypeInfo.GetCustomAttribute<AreaAttribute>() != null)
            {
                areaName = controllerAction.ControllerTypeInfo.GetCustomAttribute<AreaAttribute>().RouteValue;
            }
            if (areaName == null)
            {
                areaName = string.Empty;
            }

            bool hasPermission = HasPermission(areaName, controllerAction.ControllerName, roles);
            if (!hasPermission)
            {
                throw new ForbiddenException("You do not have permission to access this resource.");
            }
        }

        public bool HasPermission(string areaName, string actionName, List<UserRole>? userRole)
        {
            bool result = false;

            foreach (var user in userRole)
            {
                bool hasPermission = this.permissionService.CheckActionPermission(Convert.ToInt32(user.RoleId), areaName, actionName).Result;
                if (hasPermission)
                    result = hasPermission;
            }

            return result;
        }

        private bool IsControllerAllowAnonymous(Type? controllerType)
        {
            return controllerType?.GetCustomAttribute<AllowAnonymousAttribute>() != null;
        }

        public class ForbiddenException : Exception
        {
            public ForbiddenException(string message) : base(message)
            {
                StatusCode = (int)HttpStatusCode.Forbidden;
            }

            public int StatusCode { get; } = (int)HttpStatusCode.InternalServerError;
        }


    }
}
