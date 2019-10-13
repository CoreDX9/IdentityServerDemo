using System;
using CoreDX.Domain.Core.Entity;

namespace CoreDX.Domain.Model.Entity
{
    public abstract class ManyToManyReferenceEntityBase<TIdentityKey, TIdentityUser> : ManyToManyReferenceEntityBase<TIdentityKey>
        , IManyToManyReferenceEntity<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        public virtual TIdentityUser Creator { get; set; }
    }

    public abstract class ManyToManyReferenceEntityBase<TIdentityKey> : ManyToManyReferenceEntityBase
        , IManyToManyReferenceEntity<TIdentityKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
        public virtual TIdentityKey? CreatorId { get; set; }
    }

    public abstract class ManyToManyReferenceEntityBase : IManyToManyReferenceEntity
    {
        public virtual DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
    }
}
