using System;
using CoreDX.Domain.Core.Entity;

namespace CoreDX.Domain.Model.Entity.Security
{
    /// <summary>
    /// 权限声明基类
    /// </summary>
    public abstract class PermissionDeclarationBase<TKey, TIdentityKey> : ManyToManyReferenceEntityBase<TIdentityKey>
        , IOptimisticConcurrencySupported
        , ILastModificationTimeRecordable
        where TKey : struct, IEquatable<TKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
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
        public virtual PermissionDefinition<TKey, TIdentityKey> PermissionDefinition { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset LastModificationTime { get; set; } = DateTimeOffset.Now;
        public TIdentityKey? LastModifierId { get; set; }
    }

    /// <summary>
    /// 角色权限声明
    /// </summary>
    public abstract class RolePermissionDeclaration<TKey, TIdentityKey> : PermissionDeclarationBase<TKey, TIdentityKey>
        where TKey : struct, IEquatable<TKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public TKey? RoleId { get; set; }
    }

    /// <summary>
    /// 用户权限声明
    /// </summary>
    public abstract class UserPermissionDeclaration<TKey, TIdentityKey> : PermissionDeclarationBase<TKey, TIdentityKey>
        where TKey : struct, IEquatable<TKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public TKey? UserId { get; set; }
    }

    /// <summary>
    /// 组织权限声明
    /// </summary>
    public abstract class OrganizationPermissionDeclaration<TKey, TIdentityKey> : PermissionDeclarationBase<TKey, TIdentityKey>
        where TKey : struct, IEquatable<TKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
        /// <summary>
        /// 组织Id
        /// </summary>
        public TKey? OrganizationId { get; set; }
    }
}
