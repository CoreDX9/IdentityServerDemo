using System;
using System.Collections.Generic;
using Domain.Identity;

namespace Domain.Security
{
    public abstract class PermissionDeclarationBase : DomainEntityBase<Guid, Guid>
    {
        public virtual short PermissionValue { get; set; }

        public virtual Guid? PermissionDefinitionId { get; set; }

        public virtual PermissionDefinition PermissionDefinition { get; set; }
    }

    public class RolePermissionDeclaration : PermissionDeclarationBase
    {
        public Guid? RoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }
    }

    public class UserPermissionDeclaration : PermissionDeclarationBase
    {
        public Guid? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    public class OrganizationPermissionDeclaration : PermissionDeclarationBase
    {
        public Guid? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        public Guid? ParentPermissionDeclarationId { get; set; }

        public virtual OrganizationPermissionDeclaration ParentPermissionDeclaration { get; set; }
    }

    public class RequestHandlerPermissionDeclaration : PermissionDeclarationBase
    {
        public Guid? RuleId { get; set; }

        public virtual RequestAuthorizationRule Rule { get; set; }

        public virtual List<RequestHandlerPermissionDeclarationRole> PermissionDeclarationRoles { get; set; } = new List<RequestHandlerPermissionDeclarationRole>();

        public virtual List<RequestHandlerPermissionDeclarationOrganization> PermissionDeclarationOrganizations { get;
            set;
        } = new List<RequestHandlerPermissionDeclarationOrganization>();
    }
}
