using System;
using CoreDX.Application.Domain.Model.Entity.Core;
using Microsoft.AspNetCore.Identity;

namespace CoreDX.Application.Domain.Model.Entity.Identity
{
    public class ApplicationUserRole : ApplicationUserRole<Guid, ApplicationUser, ApplicationRole>
        , IStorageOrderRecordable
    {
        public virtual long InsertOrder { get; set; }
    }

    public abstract class ApplicationUserRole<TIdentityKey, TIdentityUser, TIdentityRole> : IdentityUserRole<TIdentityKey>
        , IManyToManyReferenceEntity<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
        where TIdentityRole : IEntity<TIdentityKey>
    {
        #region 导航属性

        public virtual TIdentityUser User { get; set; }
        public virtual TIdentityRole Role { get; set; }

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
