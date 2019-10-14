using System;
using CoreDX.Domain.Core.Entity;
using Microsoft.AspNetCore.Identity;

namespace CoreDX.Domain.Entity.Identity
{
    public class ApplicationUserLogin : ApplicationUserLogin<int, ApplicationUser>
    {
    }

    public abstract class ApplicationUserLogin<TKey, TIdentityUser> : IdentityUserLogin<TKey>
        , IEntity
        , ICreationTimeRecordable
        , ICreatorRecordable<TKey, TIdentityUser>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
    {
        #region 导航属性

        public virtual TIdentityUser User { get; set; }

        #endregion

        #region IEntity成员

        public virtual DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;

        #endregion

        #region IDomainEntity成员

        public virtual TKey? CreatorId { get; set; }
        public virtual TIdentityUser Creator { get; set; }

        #endregion
    }
}
