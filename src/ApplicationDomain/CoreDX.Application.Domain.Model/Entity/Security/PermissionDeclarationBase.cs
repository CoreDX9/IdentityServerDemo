using System;
using CoreDX.Application.Domain.Model.Entity.Core;
using CoreDX.Application.Domain.Model.Entity.Identity;

namespace CoreDX.Application.Domain.Model.Entity.Security
{
    /// <summary>
    /// 权限声明基类
    /// </summary>
    public abstract class PermissionDeclarationBase<TIdentityKey, TIdentityUser> : ManyToManyReferenceEntityBase<TIdentityKey, TIdentityUser>
        , IOptimisticConcurrencySupported
        , ILastModificationTimeRecordable
        ,ILastModifierRecordable<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        /// <summary>
        /// 权限值
        /// </summary>
        public virtual short PermissionValue { get; set; }

        /// <summary>
        /// 权限定义Id
        /// </summary>
        public virtual TIdentityKey? PermissionDefinitionId { get; set; }

        /// <summary>
        /// 权限定义
        /// </summary>
        public virtual PermissionDefinition<TIdentityKey, TIdentityUser> PermissionDefinition { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset LastModificationTime { get; set; } = DateTimeOffset.Now;
        public TIdentityKey? LastModifierId { get; set; }
        public TIdentityUser LastModifier { get; set; }
    }

    /// <summary>
    /// 角色权限声明
    /// </summary>
    public abstract class RolePermissionDeclaration<TKey, TIdentityUser, TIdentityRole> : PermissionDeclarationBase<TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
        where TIdentityRole : IEntity<TKey>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public TKey? RoleId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public virtual TIdentityRole Role { get; set; }
    }

    /// <summary>
    /// 用户权限声明
    /// </summary>
    public abstract class UserPermissionDeclaration<TKey, TIdentityUser> : PermissionDeclarationBase<TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public TKey? UserId { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public virtual TIdentityUser User { get; set; }
    }

    /// <summary>
    /// 组织权限声明
    /// </summary>
    public abstract class OrganizationPermissionDeclaration<TKey, TIdentityUser> : PermissionDeclarationBase<TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
    {
        /// <summary>
        /// 组织Id
        /// </summary>
        public TKey? OrganizationId { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public virtual Organization<TKey, TIdentityUser> Organization { get; set; }
    }
}
