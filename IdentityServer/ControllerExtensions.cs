using Microsoft.AspNetCore.Mvc;

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
            var headerKey = "x-requested-with";

            return controller.HttpContext.Request.Headers.ContainsKey(headerKey)
                   && controller.HttpContext.Request.Headers[headerKey] == "XMLHttpRequest";
        }
    }
}
