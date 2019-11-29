using System;
using CoreDX.Domain.Core.Entity;
using Microsoft.AspNetCore.Identity;

namespace CoreDX.Domain.Entity.Identity
{
    public class ApplicationUserToken : ApplicationUserToken<int, ApplicationUser>
    {
    }

    public abstract class ApplicationUserToken<TIdentityKey, TIdentityUser> : IdentityUserToken<TIdentityKey>
        , IEntity
        , ICreationTimeRecordable
        , ICreatorRecordable<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        #region 重写基类属性使属性变更通知事件生效

        public override TIdentityKey UserId { get => base.UserId; set => base.UserId = value; }
        public override string LoginProvider { get => base.LoginProvider; set => base.LoginProvider = value; }
        public override string Name { get => base.Name; set => base.Name = value; }
        public override string Value { get => base.Value; set => base.Value = value; }

        #endregion
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
