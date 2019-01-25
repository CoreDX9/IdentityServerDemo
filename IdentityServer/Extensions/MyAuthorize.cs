using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //如果发现匿名访问特性直接返回（context.Result == null 时直接返回表示授权通过）
            if (context.Filters.Any(f => f is AllowAnonymousFilter)) return;

            //否则没有用户名表示没有登录，返回登陆跳转
            if (context.HttpContext?.User?.Identity?.Name == null)
            {
                context.Result = new ChallengeResult();
                return;
            }

            //在不访问数据库的情况下获取部分User信息
            //var userId = context.HttpContext.User.GetSubjectId();
            //在这里判断权限，对于Razor页面，是在匹配Handler之前就进来了，需要读取QueryString的handler的值进行进一步判断
            //并且handler匹配失败会跳转回普通的OnGet去处理，需要注意

            #region 示例，判断用户名并决定是否授权，其他规则自行实现

            if (context.HttpContext?.User?.Identity?.Name?.ToLower() == "coredx") return;

            #endregion

            //到最后都没有通过授权表示授权失败，返回阻止访问（未登录跳转已经在上面了，到这里肯定已经登录了）
            context.Result = new ForbidResult();
        }
    }
}
