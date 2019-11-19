using System;
using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Model.Entity;

namespace CoreDX.Domain.Entity.Identity
{
    public class ApplicationUserOrganization : ApplicationUserOrganization<int, ApplicationUser, Organization, ApplicationUserOrganization>
    {
    }

    public class ApplicationUserOrganization<TIdentityKey, TIdentityUser, TOrganization, TUserOrganization> : ManyToManyReferenceEntityBase<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
        where TOrganization : Organization<TIdentityKey, TOrganization, TIdentityUser, TUserOrganization>
        where TUserOrganization : ApplicationUserOrganization<TIdentityKey, TIdentityUser, TOrganization, TUserOrganization>
    {
        public virtual TIdentityKey UserId { get; set; }

        public virtual TIdentityUser User { get; set; }

        public virtual TIdentityKey OrganizationId { get; set; }

        public virtual TOrganization Organization { get; set; }
    }
}
