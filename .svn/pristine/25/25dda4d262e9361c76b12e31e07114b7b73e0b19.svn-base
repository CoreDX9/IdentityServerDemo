using System.Net;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// 基于JQuery Ajax的RequestHeader x-requested-with判断是否为ajax请求
        /// </summary>
        /// <returns></returns>
        public static bool IsAjaxRequest(this Controller controller)
        {
            return IsAjaxRequest(controller.HttpContext);
        }

        public static bool IsAjaxRequest(this PageModel page)
        {
            return IsAjaxRequest(page.HttpContext);
        }

        private static bool IsAjaxRequest(this HttpContext context)
        {
            var headerKey = "x-requested-with";

            return context.Request.Headers.ContainsKey(headerKey)
                   && context.Request.Headers[headerKey] == "XMLHttpRequest";
        }

        public static string RequestReferer(this HttpContext context)
        {
            return context.Request.Headers[HttpRequestHeader.Referer.ToString()].IsNullOrEmpty()
                ? null
                : context.Request.Headers[HttpRequestHeader.Referer.ToString()].ToString();
        }
    }
}
