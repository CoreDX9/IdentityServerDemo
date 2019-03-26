using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Domain.Security;

namespace Domain.Identity
{
    public class Organization : DomainTreeEntityBase<Guid, Organization, Guid>
    {
        /// <summary>
        /// 需要使用.Include(o => o.UserOrganizations).ThenInclude(uo => uo.User)预加载或启用延迟加载
        /// </summary>
        [NotMapped]
        public virtual IEnumerable<ApplicationUser> Users => UserOrganizations?.Select(uo => uo.User);

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<OrganizationPermissionDeclaration> PermissionDeclarations { get; set; } = new List<OrganizationPermissionDeclaration>();

        public virtual List<ApplicationUserOrganizations> UserOrganizations { get; set; } =
            new List<ApplicationUserOrganizations>();
    }
}
