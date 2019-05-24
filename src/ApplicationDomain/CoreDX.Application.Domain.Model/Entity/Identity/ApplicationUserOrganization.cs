using System;
using CoreDX.Application.Domain.Model.Entity.Core;

namespace CoreDX.Application.Domain.Model.Entity.Identity
{
    public class ApplicationUserOrganization : ApplicationUserOrganization<Guid, ApplicationUser>
    {
    }

    public class ApplicationUserOrganization<TIdentityKey, TIdentityUser> : ManyToManyReferenceEntityBase<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        public TIdentityKey UserId { get; set; }

        public virtual TIdentityUser User { get; set; }

        public virtual TIdentityKey OrganizationId { get; set; }

        public virtual Organization<TIdentityKey, TIdentityUser> Organization { get; set; }
    }
}
