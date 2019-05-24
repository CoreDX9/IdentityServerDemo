using System;
using CoreDX.Application.Domain.Model.Entity.Core;
using Microsoft.AspNetCore.Identity;

namespace CoreDX.Application.Domain.Model.Entity.Identity
{
    public class ApplicationUserToken : ApplicationUserToken<Guid, ApplicationUser>
    {
    }

    public abstract class ApplicationUserToken<TIdentityKey, TIdentityUser> : IdentityUserToken<TIdentityKey>
        , IEntity
        , ICreationTimeRecordable
        , ICreatorRecordable<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        #region 导航属性

        public virtual TIdentityUser User { get; set; }

        #endregion

        #region IEntity成员

        public virtual DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;

        #endregion

        #region IDomainEntity成员

        public virtual TIdentityKey? CreatorId { get; set; }
        public virtual TIdentityUser Creator { get; set; }

        #endregion
    }
}
