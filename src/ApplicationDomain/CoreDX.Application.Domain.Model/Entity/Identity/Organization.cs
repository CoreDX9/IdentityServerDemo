using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CoreDX.Application.Domain.Model.Entity.Core;
using CoreDX.Application.Domain.Model.Entity.Security;

namespace CoreDX.Application.Domain.Model.Entity.Identity
{
    public class Organization : Organization<Guid, Guid, Guid>
    , IStorageOrderRecordable
    {
        public long InsertOrder { get; set; }
    }

    public abstract class Organization<TKey, TIdentityUserKey, TIdentityRoleKey> : DomainTreeEntityBase<TKey, Organization<TKey, TIdentityUserKey, TIdentityRoleKey>, TIdentityUserKey, TIdentityRoleKey>
        where TKey : struct, IEquatable<TKey>
        where TIdentityUserKey : struct, IEquatable<TIdentityUserKey>
        where TIdentityRoleKey : struct, IEquatable<TIdentityRoleKey>
    {
        /// <summary>
        /// 需要使用.Include(o => o.UserOrganizations).ThenInclude(uo => uo.User)预加载或启用延迟加载
        /// </summary>
        [NotMapped]
        public virtual IEnumerable<ApplicationUser<TIdentityUserKey, TIdentityRoleKey, TKey>> Users => UserOrganizations?.Select(uo => uo.User);

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<OrganizationPermissionDeclaration> PermissionDeclarations { get; set; } = new List<OrganizationPermissionDeclaration>();

        public virtual List<ApplicationUserOrganization<TKey, TIdentityUserKey, TIdentityRoleKey>> UserOrganizations { get; set; } =
            new List<ApplicationUserOrganization<TKey, TIdentityUserKey, TIdentityRoleKey>>();
    }
}
