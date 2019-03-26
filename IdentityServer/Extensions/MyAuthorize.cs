using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain.Identity;
using Domain.Security;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
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
            var dbContext =
                context.HttpContext.RequestServices.GetRequiredService<ApplicationIdentityDbContext>();

            if (context.ActionDescriptor is ControllerActionDescriptor cad)
            {
                var key =
                    (cad.MethodInfo.GetCustomAttribute(typeof(RequestHandlerIdentificationAttribute)) as
                        RequestHandlerIdentificationAttribute)?.UniqueKey;
                RequestAuthorizationRule.AuthorizationRuleGroup rule;
                if (!key.IsNullOrEmpty())
                {
                    rule = dbContext.RequestAuthorizationRules.SingleOrDefault(r => r.IdentificationKey == key)?.AuthorizationRuleConfig;
                }
                else
                {
                    var sign = cad.MethodInfo.ToString();
                    var typeName = cad.MethodInfo.DeclaringType.FullName;
                    rule = dbContext.RequestAuthorizationRules.SingleOrDefault(r => r.TypeFullName == typeName && r.HandlerMethodSignature == sign)?.AuthorizationRuleConfig;
                }

                if (rule != null)
                {
                    var isValid = Validate(rule, dbContext, Guid.Parse(context.HttpContext.User.GetSubjectId()));

                    if (isValid)
                    {
                        await Task.CompletedTask;
                        return;
                    }
                }
            }

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
                    ApplicationUser user;
                    switch (origin.Type)
                    {
                        case RequestAuthorizationRule.AuthorizationRule.PermissionOrigin.PermissionOriginType.User:
                            var upd = db.UserPermissionDeclarations.AsNoTracking().SingleOrDefault(o =>
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
                            user = db.Users.AsNoTracking()
                                .Include(u => u.UserRoles)
                                .ThenInclude(ur => ur.Role)
                                .ThenInclude(r => r.PermissionDeclarations)
                                .Single(u => u.Id == userId);//查询用户及其角色权限信息

                            //循环所有用户角色
                            foreach (var role in user.Roles)
                            {
                                var tmpRole = role;
                                var found = false;

                                while (tmpRole != null)//循环查找某个角色或其上层角色是否与某个角色来源要求匹配
                                {
                                    if (origin.Values?.Any() != true || origin.Values.Contains(tmpRole.Id))//如果找到，就设置flag并跳出循环
                                    {
                                        found = true;
                                        break;
                                    }

                                    tmpRole = tmpRole.Parent;
                                }

                                if (found)//如果找到匹配角色，查找角色或其上层角色是否有相应权限
                                {
                                    tmpRole = role;
                                    while (tmpRole != null)
                                    {
                                        var permissionDeclaration =
                                            tmpRole.PermissionDeclarations.SingleOrDefault(pd1 =>
                                                pd1.PermissionDefinitionId == rule.PermissionDefinitionId);

                                        if (permissionDeclaration != null)
                                        {
                                            switch (pd.ValueType)
                                            {
                                                case PermissionValueType.Boolean:
                                                    if (permissionDeclaration.PermissionValue > 0)
                                                    {
                                                        originRuleTrue = true;
                                                    }

                                                    break;
                                                case PermissionValueType.Number:
                                                    if (permissionDeclaration.PermissionValue >= rule.Value)
                                                    {
                                                        originRuleTrue = true;
                                                    }

                                                    break;
                                                default:
                                                    throw new ArgumentOutOfRangeException();
                                            }

                                            if (originRuleTrue)
                                            {
                                                break;
                                            }
                                        }

                                        tmpRole = tmpRole.Parent;
                                    }

                                    if (originRuleTrue)
                                    {
                                        break;
                                    }
                                }
                            }

                            break;
                        case RequestAuthorizationRule.AuthorizationRule.PermissionOrigin.PermissionOriginType
                            .Organization:
                            user = db.Users.AsNoTracking()
                                .Include(u => u.UserOrganizations)
                                .ThenInclude(uo => uo.Organization)
                                .ThenInclude(r => r.PermissionDeclarations)
                                .Single(u => u.Id == userId);//查询用户及其组织权限信息

                            //循环所有用户组织
                            foreach (var organization in user.Organizations)
                            {
                                var tmpOrganization = organization;
                                var found = false;

                                while (tmpOrganization != null)//循环查找某个组织或其上层组织是否与某个组织来源要求匹配
                                {
                                    if (origin.Values?.Any() != true || origin.Values.Contains(tmpOrganization.Id))//如果找到，就设置flag并跳出循环
                                    {
                                        found = true;
                                        break;
                                    }

                                    tmpOrganization = tmpOrganization.Parent;
                                }

                                if (found)//如果找到匹配组织，查找组织是否有相应权限
                                {
                                    var permissionDeclaration =
                                        organization.PermissionDeclarations.SingleOrDefault(pd1 =>
                                            pd1.PermissionDefinitionId == rule.PermissionDefinitionId);

                                    if (permissionDeclaration != null)
                                    {
                                        switch (pd.ValueType)
                                        {
                                            case PermissionValueType.Boolean:
                                                if (permissionDeclaration.PermissionValue > 0)
                                                {
                                                    originRuleTrue = true;
                                                }

                                                break;
                                            case PermissionValueType.Number:
                                                if (permissionDeclaration.PermissionValue >= rule.Value)
                                                {
                                                    originRuleTrue = true;
                                                }

                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }

                                        if (originRuleTrue)
                                        {
                                            break;
                                        }
                                    }
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
