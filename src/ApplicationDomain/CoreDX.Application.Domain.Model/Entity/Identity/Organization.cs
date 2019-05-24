using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CoreDX.Application.Domain.Model.Entity.Core;
using CoreDX.Application.Domain.Model.Entity.Security;

namespace CoreDX.Application.Domain.Model.Entity.Identity
{
    public class Organization : Organization<Guid, ApplicationUser>
    , IStorageOrderRecordable
    {
        public long InsertOrder { get; set; }
    }

    public abstract class Organization<TKey, TIdentityUser> : DomainTreeEntityBase<Organization<TKey, TIdentityUser>>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUser : IEntity<TKey>
    {
        /// <summary>
        /// 需要使用.Include(o => o.UserOrganizations).ThenInclude(uo => uo.User)预加载或启用延迟加载
        /// </summary>
        [NotMapped]
        public virtual IEnumerable<TIdentityUser> Users => UserOrganizations?.Select(uo => uo.User);

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<OrganizationPermissionDeclaration<TKey, TIdentityUser>> PermissionDeclarations { get; set; } = new List<OrganizationPermissionDeclaration<TKey, TIdentityUser>>();

        public virtual List<ApplicationUserOrganization<TKey, TIdentityUser>> UserOrganizations { get; set; } =
            new List<ApplicationUserOrganization<TKey, TIdentityUser>>();
    }
}
