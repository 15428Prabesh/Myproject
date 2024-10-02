using Core.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper.Extensions
{
    public static class MessageExtension
    {
        /// <summary>
        /// Adds the message in current pipeline of HttpContext
        /// </summary>
        /// <param name="httpContext">httpContext</param>
        /// <param name="message">message  to be shown</param>
        /// <param name="messageType">Type of message : MessageType </param>
        public static void ShowMessage(this IHttpContextAccessor httpContextAccessor, string message, MessageType messageType)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                httpContextAccessor.HttpContext.Items[HttpContextKey.ActionMessage] = message;
                httpContextAccessor.HttpContext.Items[HttpContextKey.ActionMessageType] = messageType.ToString();
            }
        }
    }
}
