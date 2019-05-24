using System;
using Microsoft.AspNetCore.Identity;

namespace CoreDX.Application.Domain.Model.Entity.Core
{
    public interface IManyToManyReferenceEntity<TIdentityKey, TIdentityUser> : IManyToManyReferenceEntity
        , ICreatorRecordable<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
    }

    public interface IManyToManyReferenceEntity : IEntity
        , ICreationTimeRecordable
    {
    }
}
