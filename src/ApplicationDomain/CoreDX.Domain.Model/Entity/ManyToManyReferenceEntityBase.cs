using System;
using CoreDX.Domain.Core.Entity;

namespace CoreDX.Domain.Model.Entity
{
    /// <summary>
    /// 多对多导航实体基类
    /// </summary>
    /// <typeparam name="TIdentityKey">身份实体主键类型</typeparam>
    /// <typeparam name="TIdentityUser">身份实体类型</typeparam>
    public abstract class ManyToManyReferenceEntityBase<TIdentityKey, TIdentityUser> : ManyToManyReferenceEntityBase<TIdentityKey>
        , IManyToManyReferenceEntity<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        public virtual TIdentityUser Creator { get; set; }
    }

    /// <summary>
    /// 多对多导航实体基类
    /// </summary>
    /// <typeparam name="TIdentityKey">身份实体主键类型</typeparam>
    public abstract class ManyToManyReferenceEntityBase<TIdentityKey> : ManyToManyReferenceEntityBase
        , IManyToManyReferenceEntity<TIdentityKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
        public virtual TIdentityKey? CreatorId { get; set; }
    }

    /// <summary>
    /// 多对多导航实体基类
    /// </summary>
    public abstract class ManyToManyReferenceEntityBase : IManyToManyReferenceEntity
    {
        public virtual DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
    }
}
