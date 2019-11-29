using System;
using CoreDX.Domain.Core.Entity;
using Microsoft.AspNetCore.Identity;

namespace CoreDX.Domain.Entity.Identity
{
    public class ApplicationUserRole : ApplicationUserRole<int, ApplicationUser, ApplicationRole>
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
        #region 重写基类属性使属性变更通知事件生效

        public override TIdentityKey UserId { get => base.UserId; set => base.UserId = value; }
        public override TIdentityKey RoleId { get => base.RoleId; set => base.RoleId = value; }

        #endregion

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
