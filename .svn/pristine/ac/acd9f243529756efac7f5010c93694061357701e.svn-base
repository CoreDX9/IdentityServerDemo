using System;
using System.Collections.Generic;
using System.Text;
using Domain.Security;

namespace Domain.Identity
{
    public class Organization : DomainTreeEntityBase<Guid, Organization, Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<OrganizationPermissionDeclaration> PermissionDeclarations { get; set; } = new List<OrganizationPermissionDeclaration>();
    }
}
