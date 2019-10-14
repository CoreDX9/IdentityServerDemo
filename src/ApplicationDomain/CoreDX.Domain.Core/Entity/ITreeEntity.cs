using System;

namespace CoreDX.Domain.Core.Entity
{
    /// <summary>
    /// 树形实体接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface ITreeEntity<T> : IEntity, ITree<T>
    {
    }

    /// <summary>
    /// 树形实体接口
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface ITreeEntity<TKey, TEntity> : ITreeEntity<TEntity>, IEntity<TKey>
    where TKey : IEquatable<TKey>
    where TEntity : ITreeEntity<TKey, TEntity>
    {
    }
}
