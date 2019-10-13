using System;

namespace CoreDX.Domain.Core.Entity
{
    public interface IManyToManyReferenceEntity<TIdentityKey, TIdentityUser> : IManyToManyReferenceEntity<TIdentityKey>
        , ICreatorRecordable<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
    }

    public interface IManyToManyReferenceEntity<TIdentityKey> : IManyToManyReferenceEntity
        , ICreatorRecordable<TIdentityKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
    }

    public interface IManyToManyReferenceEntity : IEntity
        , ICreationTimeRecordable
    {
    }
}
