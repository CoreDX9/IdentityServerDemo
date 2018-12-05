using System;
using System.Collections.Generic;
using Domain.Identity;

namespace Domain.Security
{
    public class RequestAuthorizationRule : DomainEntityBase<Guid, Guid>
    {
        public string MethodSignName { get; set; }

        public string TypeFullName { get; set; }

        public string FriendlyName { get; set; }

        public string AdvanceAuthorizationRuleJson { get; set; }

        public virtual List<RequestHandlerPermissionDeclaration> PermissionDeclarations { get; set; } = new List<RequestHandlerPermissionDeclaration>();
    }

    public class RequestHandlerPermissionDeclarationRole : DomainEntityBase<Guid, Guid>
    {
        public Guid? RoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public Guid? PermissionDeclarationId { get; set; }

        public virtual RequestHandlerPermissionDeclaration PermissionDeclaration { get; set; }
    }

    public class RequestHandlerPermissionDeclarationOrganization : DomainEntityBase<Guid, Guid>
    {
        public Guid? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        public Guid? PermissionDeclarationId { get; set; }

        public virtual RequestHandlerPermissionDeclaration PermissionDeclaration { get; set; }
    }
}
