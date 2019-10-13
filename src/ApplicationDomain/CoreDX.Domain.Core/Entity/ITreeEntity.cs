using System;

namespace CoreDX.Domain.Core.Entity
{
    public interface ITreeEntity<T> : IEntity, ITree<T>
    {
    }

    public interface ITreeEntity<TKey, TEntity> : ITreeEntity<TEntity>, IEntity<TKey>
    where TKey : IEquatable<TKey>
    where TEntity : ITreeEntity<TKey, TEntity>
    {
    }
}
