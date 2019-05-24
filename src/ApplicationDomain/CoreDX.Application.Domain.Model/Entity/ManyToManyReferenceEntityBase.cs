using System;
using CoreDX.Application.Domain.Model.Entity.Core;
using Microsoft.AspNetCore.Identity;

namespace CoreDX.Application.Domain.Model.Entity
{
    public abstract class ManyToManyReferenceEntityBase<TIdentityKey, TIdentityUser> : ManyToManyReferenceEntityBase
        , IManyToManyReferenceEntity<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        #region IDomainEntity成员

        public virtual TIdentityKey? CreatorId { get; set; }
        public virtual TIdentityUser Creator { get; set; }

        #endregion
    }

    public abstract class ManyToManyReferenceEntityBase : IManyToManyReferenceEntity
    {
        public virtual DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
    }
}
