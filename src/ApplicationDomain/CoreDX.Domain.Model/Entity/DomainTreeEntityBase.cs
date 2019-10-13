using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreDX.Domain.Core.Entity;
using PropertyChanged;

namespace CoreDX.Domain.Model.Entity
{
    /// <summary>
    /// 确定了实体操作人主键类型的树形领域实体基类
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TIdentityKey">Identity主键类型</typeparam>
    /// <typeparam name="TIdentityUser">Identity实体类型</typeparam>
    public abstract class DomainTreeEntityBase<TKey, TEntity, TIdentityKey, TIdentityUser> : DomainTreeEntityBase<TKey, TEntity, TIdentityKey>
        , ICreatorRecordable<TIdentityKey, TIdentityUser>
        , ILastModifierRecordable<TIdentityKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TEntity : DomainTreeEntityBase<TKey, TEntity, TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        #region IDomainEntity成员

        public virtual TIdentityUser Creator { get; set; }
        public virtual TIdentityUser LastModifier { get; set; }

        #endregion
    }

    public abstract class DomainTreeEntityBase<TKey, TEntity, TIdentityKey> : DomainTreeEntityBase<TKey, TEntity>
        , ICreatorRecordable<TIdentityKey>
        , ILastModifierRecordable<TIdentityKey>
        where TKey : struct, IEquatable<TKey>
        where TEntity : DomainTreeEntityBase<TKey, TEntity, TIdentityKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
        public virtual TIdentityKey? CreatorId { get; set; }
        public virtual TIdentityKey? LastModifierId { get; set; }
    }

    public abstract class DomainTreeEntityBase<TKey, TEntity> : DomainEntityBase<TKey>
        , IDomainTreeEntity<TKey, TEntity>
        where TKey : struct, IEquatable<TKey>
        where TEntity : DomainTreeEntityBase<TKey, TEntity>
    {
        #region ITree成员

        [ForeignKey(nameof(ParentId))]
        public virtual TEntity Parent { get; set; }
        public virtual IList<TEntity> Children { get; set; } = new List<TEntity>();

        [DoNotNotify, NotMapped]
        public virtual int Depth => Parent?.Depth + 1 ?? 0;

        [DoNotNotify, NotMapped]
        public virtual bool IsRoot => Parent == null;

        [DoNotNotify, NotMapped]
        public virtual bool IsLeaf => Children?.Count == 0;

        [DoNotNotify, NotMapped]
        public virtual bool HasChildren => !IsLeaf;

        [DoNotNotify, NotMapped]
        public virtual string Path => Parent == null ? Id.ToString() : $@"{Parent.Path}/{Id}";

        #endregion

        #region IDomainTreeEntity成员

        public virtual TKey? ParentId { get; set; }

        #endregion
    }
}
