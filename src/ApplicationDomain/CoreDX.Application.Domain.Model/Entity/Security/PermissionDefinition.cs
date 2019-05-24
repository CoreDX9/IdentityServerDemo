using System;
using CoreDX.Application.Domain.Model.Entity.Core;
using CoreDX.Application.Domain.Model.Entity.Identity;
using CoreDX.EntityFrameworkCore.Extensions.DataAnnotations;

namespace CoreDX.Application.Domain.Model.Entity.Security
{
    [DbDescription("权限值类型，物理存储都是数字")]
    public enum PermissionValueType : sbyte
    {
        [DbDescription("布尔型，表示权限只存在有和没有两种取值，大于零为有，否则为没有")]
        Boolean = 1,
        [DbDescription("整数型，大于零的整数越大表示权限越高，否则表示没有权限")]
        Number = 2
    }

    /// <summary>
    /// 权限定义
    /// </summary>
    public class PermissionDefinition : PermissionDefinition<Guid, ApplicationUser>
    {
    }

    /// <summary>
    /// 权限定义
    /// </summary>
    public class PermissionDefinition<TKey, TIdentityUser> : DomainEntityBase<TKey, TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 值类型
        /// </summary>
        public PermissionValueType ValueType { get; set; }
    }
}
