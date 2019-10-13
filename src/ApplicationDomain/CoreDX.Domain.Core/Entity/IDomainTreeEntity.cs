using System;

namespace CoreDX.Domain.Core.Entity
{
    public interface IDomainTreeEntity<T> : IDomainEntity, ITreeEntity<T> {}

    public interface IDomainTreeEntity<TKey, TEntity> : IDomainTreeEntity<TEntity>, IDomainEntity<TKey>, ITreeEntity<TKey, TEntity>
        where TKey : struct, IEquatable<TKey>
        where TEntity : IDomainTreeEntity<TKey, TEntity>
    {
        TKey? ParentId { get; set; }
    }
}
