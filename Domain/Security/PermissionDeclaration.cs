using System;
using System.Collections.Generic;
using Domain.Identity;

namespace Domain.Security
{
    /// <summary>
    /// 权限声明基类
    /// </summary>
    public abstract class PermissionDeclarationBase : DomainEntityBase<Guid, Guid>
    {
        /// <summary>
        /// 权限值
        /// </summary>
        public virtual short PermissionValue { get; set; }

        /// <summary>
        /// 权限定义Id
        /// </summary>
        public virtual Guid? PermissionDefinitionId { get; set; }

        /// <summary>
        /// 权限定义
        /// </summary>
        public virtual PermissionDefinition PermissionDefinition { get; set; }
    }

    /// <summary>
    /// 角色权限声明
    /// </summary>
    public class RolePermissionDeclaration : PermissionDeclarationBase
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid? RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public virtual ApplicationRole Role { get; set; }
    }

    /// <summary>
    /// 用户权限声明
    /// </summary>
    public class UserPermissionDeclaration : PermissionDeclarationBase
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public virtual ApplicationUser User { get; set; }
    }

    /// <summary>
    /// 组织权限声明
    /// </summary>
    public class OrganizationPermissionDeclaration : PermissionDeclarationBase
    {
        /// <summary>
        /// 组织Id
        /// </summary>
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public virtual Organization Organization { get; set; }

        #region 回头删掉
        //todo:回头删掉
        //public Guid? ParentPermissionDeclarationId { get; set; }

        //public virtual OrganizationPermissionDeclaration ParentPermissionDeclaration { get; set; }

        #endregion
    }

    //todo:回头看看怎么处理，估计是要删掉
    //public class RequestHandlerPermissionDeclaration : PermissionDeclarationBase
    //{
    //    public Guid? RuleId { get; set; }

    //    public virtual RequestAuthorizationRule Rule { get; set; }

    //    public virtual List<RequestHandlerPermissionDeclarationRole> PermissionDeclarationRoles { get; set; } = new List<RequestHandlerPermissionDeclarationRole>();

    //    public virtual List<RequestHandlerPermissionDeclarationOrganization> PermissionDeclarationOrganizations { get;
    //        set;
    //    } = new List<RequestHandlerPermissionDeclarationOrganization>();
    //}
}
