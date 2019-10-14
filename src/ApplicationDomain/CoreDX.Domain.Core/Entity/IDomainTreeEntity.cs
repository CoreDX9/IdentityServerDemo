using System;

namespace CoreDX.Domain.Core.Entity
{
    /// <summary>
    /// 树形领域实体接口
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IDomainTreeEntity<T> :
        IDomainEntity
        , ITreeEntity<T>
    {
    }

    /// <summary>
    /// 树形领域实体接口
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TEntity">树形实体类型</typeparam>
    public interface IDomainTreeEntity<TKey, TEntity> :
        IDomainTreeEntity<TEntity>
        , IDomainEntity<TKey>
        , ITreeEntity<TKey, TEntity>

        where TKey : struct, IEquatable<TKey>
        where TEntity : IDomainTreeEntity<TKey, TEntity>
    {
        TKey? ParentId { get; set; }
    }
}
