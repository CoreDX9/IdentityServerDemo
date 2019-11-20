using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreDX.Domain.Core.Entity;

namespace CoreDX.Domain.Model.Entity
{
    /// <summary>
    /// 树形实体视图基类，包含DomainTreeEntityBase&lt;TKey, TIdentityUserKey&gt;的成员
    /// </summary>
    /// <typeparam name="TKey">主键类型（Guid主键存在string转换器的话可以在这里用string）</typeparam>
    /// <typeparam name="TEntityView">实体视图类型</typeparam>
    /// <typeparam name="TIdentityKey">IdentityUser主键类型</typeparam>
    public abstract class DomainTreeEntityViewBase<TKey, TEntityView, TIdentityKey> : DomainTreeEntityViewBase<TKey, TEntityView>
        , ICreatorRecordable<TIdentityKey>, ILastModifierRecordable<TIdentityKey>
        where TKey : struct, IEquatable<TKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TEntityView : DomainTreeEntityViewBase<TKey, TEntityView, TIdentityKey>
    {
        public virtual TIdentityKey? CreatorId { get; set; }
        public virtual TIdentityKey? LastModifierId { get; set; }
    }

    public abstract class DomainTreeEntityViewBase<TKey, TEntityView> : IDomainTreeEntity<TKey, TEntityView>
        where TKey : struct, IEquatable<TKey>
        where TEntityView : DomainTreeEntityViewBase<TKey, TEntityView>
    {
        public virtual TKey Id { get; set; }
        public virtual TKey? ParentId { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTimeOffset CreationTime { get; set; }
        public virtual DateTimeOffset LastModificationTime { get; set; }
        public virtual TEntityView Parent { get; set; }
        public virtual IList<TEntityView> Children { get; set; } = new List<TEntityView>();

        public virtual int Depth { get; set; }

        public virtual bool IsRoot => Depth == 0;

        public virtual bool IsLeaf => !HasChildren;

        public virtual bool HasChildren { get; set; }

        public virtual string Path { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool HasChanges(params string[] names)
        {
            return false;
        }

        public IDictionary<string, object> GetChanges()
        {
            return new Dictionary<string, object>();
        }

        public void ResetPropertyChangeStatus(params string[] names)
        {
            return;
        }
    }

    public abstract class DomainTreeEntityView2Base<TKey, TEntityView> : IDomainTreeEntity<TEntityView>
        where TEntityView : DomainTreeEntityView2Base<TKey, TEntityView>
    {
        public virtual TKey Id { get; set; }
        public virtual TKey ParentId { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTimeOffset CreationTime { get; set; }
        public virtual DateTimeOffset LastModificationTime { get; set; }
        public virtual TEntityView Parent { get; set; }
        public virtual IList<TEntityView> Children { get; set; } = new List<TEntityView>();

        public virtual int Depth { get; set; }

        public virtual bool IsRoot => Depth == 0;

        public virtual bool IsLeaf => !HasChildren;

        public virtual bool HasChildren { get; set; }

        public virtual string Path { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool HasChanges(params string[] names)
        {
            return false;
        }

        public IDictionary<string, object> GetChanges()
        {
            return new Dictionary<string, object>();
        }

        public void ResetPropertyChangeStatus(params string[] names)
        {
            return;
        }
    }
}
