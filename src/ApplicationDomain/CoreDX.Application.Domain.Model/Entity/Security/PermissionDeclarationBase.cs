using System;
using CoreDX.Application.Domain.Model.Entity.Identity;

namespace CoreDX.Application.Domain.Model.Entity.Security
{
    /// <summary>
    /// 权限声明基类
    /// </summary>
    public abstract class PermissionDeclarationBase<TKey> : DomainEntityBase<TKey, TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// 权限值
        /// </summary>
        public virtual short PermissionValue { get; set; }

        /// <summary>
        /// 权限定义Id
        /// </summary>
        public virtual TKey? PermissionDefinitionId { get; set; }

        /// <summary>
        /// 权限定义
        /// </summary>
        public virtual PermissionDefinition<TKey> PermissionDefinition { get; set; }
    }

    /// <summary>
    /// 角色权限声明
    /// </summary>
    public class RolePermissionDeclaration<TKey> : PermissionDeclarationBase<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public TKey? RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public virtual ApplicationRole<TKey> Role { get; set; }
    }

    /// <summary>
    /// 用户权限声明
    /// </summary>
    public class UserPermissionDeclaration<TKey> : PermissionDeclarationBase<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public TKey? UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public virtual ApplicationUser<TKey> User { get; set; }
    }

    /// <summary>
    /// 组织权限声明
    /// </summary>
    public class OrganizationPermissionDeclaration<TKey> : PermissionDeclarationBase<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// 组织Id
        /// </summary>
        public TKey? OrganizationId { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public virtual Organization<TKey> Organization { get; set; }
    }
}
