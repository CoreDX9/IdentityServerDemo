using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreDX.Application.Domain.Model.Entity.Core;
using PropertyChanged;

namespace CoreDX.Application.Domain.Model.Entity
{
    public abstract class DomainTreeEntityBase<TEntity> : DomainTreeEntityBase<Guid, TEntity, Guid, Guid>
        where TEntity : DomainTreeEntityBase<TEntity>
    {
        protected DomainTreeEntityBase()
        {
            Id = Guid.NewGuid();
        }
    }

    /// <summary>
    /// 确定了实体操作人主键类型的树形领域实体基类
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TIdentityUserKey">实体类型</typeparam>
    /// <typeparam name="TIdentityRoleKey"></typeparam>
    public abstract class DomainTreeEntityBase<TKey, TEntity, TIdentityUserKey, TIdentityRoleKey> : DomainEntityBase<TKey, TIdentityUserKey, TIdentityRoleKey>
        , IDomainTreeEntity<TKey, TEntity>
        where TKey : struct, IEquatable<TKey>
        where TEntity : DomainTreeEntityBase<TKey, TEntity, TIdentityUserKey, TIdentityRoleKey>
        where TIdentityUserKey : struct, IEquatable<TIdentityUserKey>
        where TIdentityRoleKey : struct, IEquatable<TIdentityRoleKey>
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
