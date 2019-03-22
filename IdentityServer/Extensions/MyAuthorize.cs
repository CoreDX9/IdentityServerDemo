using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain.Security;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Repository.EntityFrameworkCore.Identity;
using Util.TypeExtensions;

namespace IdentityServer.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MyAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter//可以不继承Attribute类，在配置mvc服务时加入过滤器
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //如果发现匿名访问特性直接返回（context.Result == null 时直接返回表示授权通过）
            if (context.Filters.Any(f => f is AllowAnonymousFilter)) return;

            //否则没有用户名表示没有登录，返回登陆跳转
            if (context.HttpContext?.User?.Identity?.Name == null)
            {
                context.Result = new ChallengeResult();
                await Task.CompletedTask;
                return;
            }

            //基于数据库权限信息的授权判断
            //var dbContext =
            //    context.HttpContext.RequestServices.GetRequiredService<ApplicationIdentityDbContext>();

            //if (context.ActionDescriptor is ControllerActionDescriptor cad)
            //{
            //    var key =
            //        (cad.MethodInfo.GetCustomAttribute(typeof(RequestHandlerIdentificationAttribute)) as
            //            RequestHandlerIdentificationAttribute)?.UniqueKey;
            //    RequestAuthorizationRule.AuthorizationRuleGroup rule = null;
            //    if (!key.IsNullOrEmpty())
            //    {
            //        rule = dbContext.RequestAuthorizationRules.SingleOrDefault(r => r.IdentificationKey == key)?.AuthorizationRuleConfig;
            //    }
            //    else
            //    {
            //        var sign = cad.MethodInfo.ToString();
            //        var typeName = cad.MethodInfo.DeclaringType.FullName;
            //        rule = dbContext.RequestAuthorizationRules.SingleOrDefault(r => r.TypeFullName == typeName && r.HandlerMethodSignature == sign)?.AuthorizationRuleConfig;
            //    }

            //    if (rule != null)
            //    {
            //        var result = Validate(rule, dbContext, Guid.Parse(context.HttpContext.User.GetSubjectId()));
            //    }
            //}

            //在不访问数据库的情况下获取部分User信息，此处获得的信息是通过IdentityServer扩展功能获取的，默认项目模板没有
            //var userId = context.HttpContext.User.GetSubjectId();
            //在这里判断权限，对于Razor页面，是在匹配Handler之前就进来了，需要读取QueryString的handler的值进行进一步判断
            //并且handler匹配失败会跳转回普通的OnGet去处理，需要注意

            #region 示例，判断用户名并决定是否授权，其他规则自行实现

            if (context.HttpContext?.User?.Identity?.Name?.ToLower() == "coredx")
            {
                await Task.CompletedTask;
                return;
            }

            #endregion

            //到最后都没有通过授权表示授权失败，返回阻止访问（未登录跳转已经在上面了，到这里肯定已经登录了）
            context.Result = new ForbidResult();

            await Task.CompletedTask;
        }

        private bool Validate(RequestAuthorizationRule.AuthorizationRuleGroup ruleGroup,
            ApplicationIdentityDbContext db, Guid userId)
        {
            //循环验证组中的每一条规则
            foreach (var rule in ruleGroup.Rules)
            {
                var valid = false;//规则是否验证成功
                var pd = db.PermissionDefinitions.Find(rule.PermissionDefinitionId);

                //如果没有指定权限来源，设置为任意来源
                if (rule.Origins?.Any() == false)
                {
                    rule.Origins = new List<RequestAuthorizationRule.AuthorizationRule.PermissionOrigin>
                    {
                        new RequestAuthorizationRule.AuthorizationRule.PermissionOrigin
                        {
                            Type = RequestAuthorizationRule.AuthorizationRule.PermissionOrigin.PermissionOriginType.User
                        },
                        new RequestAuthorizationRule.AuthorizationRule.PermissionOrigin
                        {
                            Type = RequestAuthorizationRule.AuthorizationRule.PermissionOrigin.PermissionOriginType.Role
                        },
                        new RequestAuthorizationRule.AuthorizationRule.PermissionOrigin
                        {
                            Type = RequestAuthorizationRule.AuthorizationRule.PermissionOrigin.PermissionOriginType.Organization
                        },
                    };
                }

                //循环在所有来源中查找匹配的权限
                foreach (var origin in rule.Origins ?? new List<RequestAuthorizationRule.AuthorizationRule.PermissionOrigin>())
                {
                    var originRuleTrue = false;//某个来源是否验证成功
                    switch (origin.Type)
                    {
                        case RequestAuthorizationRule.AuthorizationRule.PermissionOrigin.PermissionOriginType.User:
                            var upd = db.UserPermissionDeclarations.SingleOrDefault(o =>
                                o.PermissionDefinitionId == rule.PermissionDefinitionId && o.UserId == userId);

                            if (upd != null)
                            {
                                switch (pd.ValueType)
                                {
                                    case PermissionValueType.Boolean:
                                        if (upd.PermissionValue > 0)
                                        {
                                            originRuleTrue = true;
                                        }

                                        break;
                                    case PermissionValueType.Number:
                                        if (upd.PermissionValue >= rule.Value)
                                        {
                                            originRuleTrue = true;
                                        }

                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }

                            break;
                        case RequestAuthorizationRule.AuthorizationRule.PermissionOrigin.PermissionOriginType.Role:
                            foreach (var roleId in origin.Values )
                            {
                                /*todo:角色权限要继续查看上层角色是否有权限，如果子级角色存在权限表示子级角色要覆盖上层角色的权限
                                 *todo:角色权限只循环用户所在角色是来源要求角色或子角色，实际权限值以子级角色的权限值为准，前提是来源要求角色本身有相应权限或来源要求角色从其上级角色继承了权限
                                 */
                            }

                            var rpd = db.RolePermissionDeclarations.FirstOrDefault(o =>
                                o.PermissionDefinitionId == rule.PermissionDefinitionId);

                            if (rpd != null)
                            {
                                switch (pd.ValueType)
                                {
                                    case PermissionValueType.Boolean:
                                        if (rpd.PermissionValue > 0)
                                        {
                                            originRuleTrue = true;
                                        }

                                        break;
                                    case PermissionValueType.Number:
                                        if (rpd.PermissionValue >= rule.Value)
                                        {
                                            originRuleTrue = true;
                                        }

                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }

                            break;
                        case RequestAuthorizationRule.AuthorizationRule.PermissionOrigin.PermissionOriginType
                            .Organization:
                            foreach (var organizationId in origin.Values)
                            {
                                /*todo:组织权限只看用户所在组织是否有权限
                                 *todo:组织权限只循环用户所在组织是来源要求组织或子组织，实际权限值以用户所在组织的权限值为准，前提是来源要求组织本身有相应权限
                                 */
                            }

                            var opd = db.OrganizationPermissionDeclarations.FirstOrDefault(o =>
                                o.PermissionDefinitionId == rule.PermissionDefinitionId);

                            if (opd != null)
                            {
                                switch (pd.ValueType)
                                {
                                    case PermissionValueType.Boolean:
                                        if (opd.PermissionValue > 0)
                                        {
                                            originRuleTrue = true;
                                        }

                                        break;
                                    case PermissionValueType.Number:
                                        if (opd.PermissionValue >= rule.Value)
                                        {
                                            originRuleTrue = true;
                                        }

                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    //某个来源验证成功视为整条规则验证成功，停止验证其他来源
                    if (originRuleTrue)
                    {
                        valid = true;
                        break;
                    }
                }

                //如果该分组为任意规则成功并且有验证成功的，直接返回成功
                if (ruleGroup.GroupOperate == RequestAuthorizationRule.AuthorizationRuleGroup.Operate.Any &&
                    valid)
                {
                    return true;
                }

                //如果该分组为所有规则成功并且有验证失败的，直接返回失败
                if (ruleGroup.GroupOperate == RequestAuthorizationRule.AuthorizationRuleGroup.Operate.All &&
                    valid == false)
                {
                    return false;
                }
            }

            //循环验证组中的每一个子分组
            foreach (var @group in ruleGroup.Groups ?? new List<RequestAuthorizationRule.AuthorizationRuleGroup>())
            {
                var valid = Validate(@group, db, userId);

                //如果该分组为任意子分组成功并且有验证成功的，直接返回成功
                if (ruleGroup.GroupOperate == RequestAuthorizationRule.AuthorizationRuleGroup.Operate.Any &&
                    valid)
                {
                    return true;
                }

                //如果该分组为所有子分组成功并且有验证失败的，直接返回失败
                if (ruleGroup.GroupOperate == RequestAuthorizationRule.AuthorizationRuleGroup.Operate.All &&
                    valid == false)
                {
                    return false;
                }
            }

            //如果该分组为所有规则和子分组成功并且到循环结束都没有因为验证失败而提前返回失败，说明所有验证都成功，返回成功
            if (ruleGroup.GroupOperate == RequestAuthorizationRule.AuthorizationRuleGroup.Operate.All)
            {
                return true;
            }

            //如果该分组为任意规则或子分组成功并且到循环结束都没有因为验证成功而提前返回成功，说明所有验证都失败，返回失败
            //if (ruleGroup.GroupOperate == RequestAuthorizationRule.AuthorizationRuleGroup.Operate.Any)
            //{
            return false;
            //}
        }
    }
}
