using System;
using Entity;

namespace Domain
{
    /// <summary>
    /// 树形领域实体基类
    /// 提供实体创建人与最近实体修改者的存储支持
    /// </summary>
    /// <typeparam name="TParentKey">主键类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TIdentityUserKey">实体操作人的实体类型主键</typeparam>
    public interface IDomainTreeEntity<TParentKey, TEntity, TIdentityUserKey> : ITree<TEntity>, IDomainEntity<TIdentityUserKey>
        where TParentKey : struct, IEquatable<TParentKey>
        where TIdentityUserKey : struct, IEquatable<TIdentityUserKey>
        where TEntity : IDomainTreeEntity<TParentKey, TEntity, TIdentityUserKey>
    {
        TParentKey? ParentId { get; set; }
    }
}
