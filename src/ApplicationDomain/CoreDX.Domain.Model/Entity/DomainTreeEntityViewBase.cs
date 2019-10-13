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
    public class DomainTreeEntityViewBase<TKey, TEntityView, TIdentityKey> : IDomainTreeEntity<TEntityView>
        where TEntityView : DomainTreeEntityViewBase<TKey, TEntityView, TIdentityKey>
    {
        #region IEntity成员

        public virtual bool? Active { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual DateTimeOffset CreationTime { get; set; }

        public virtual DateTimeOffset LastModificationTime { get; set; }

        #endregion

        #region ITree<>成员

        public TEntityView Parent { get; set; }

        public IList<TEntityView> Children { get; set; } = new List<TEntityView>();

        /// <summary>
        /// 节点深度
        /// 由数据库视图计算
        /// </summary>
        public virtual int Depth { get; set; }

        /// <summary>
        /// 是否有子节点
        /// 由数据库视图计算
        /// </summary>
        public virtual bool HasChildren { get; set; }

        /// <summary>
        /// 节点路径
        /// 由数据库视图计算
        /// </summary>
        public virtual string Path { get; set; }

        public bool IsRoot => Depth == 0;

        public bool IsLeaf => !HasChildren;

        #endregion

        #region 模拟的IDomainEntity<,>成员

        public virtual TIdentityKey CreatorId { get; set; }

        public virtual TIdentityKey LastModifierId { get; set; }

        #endregion

        #region 模拟的DomainEntityBase<,>成员

        public virtual TKey Id { get; set; }

        #endregion

        #region 模拟的DomainTreeEntityBase<,,>成员

        public virtual TKey ParentId { get; set; }

        #endregion

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
