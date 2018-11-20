using Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EntityFrameworkCore.EntityTypeConfigurations
{
    public class PermissionDefinitionConfig : IEntityTypeConfiguration<PermissionDefinition>
    {
        public void Configure(EntityTypeBuilder<PermissionDefinition> builder)
        {
            builder.ToTable("PermissionDefinitions");
        }
    }

    public class RolePermissionDeclarationConfig : IEntityTypeConfiguration<RolePermissionDeclaration>
    {
        public void Configure(EntityTypeBuilder<RolePermissionDeclaration> builder)
        {
            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.PermissionDeclarations)
                .HasForeignKey(rp => rp.RoleId);

            builder.ToTable("RolePermissionDeclarations");
        }
    }

    public class UserPermissionDeclarationConfig : IEntityTypeConfiguration<UserPermissionDeclaration>
    {
        public void Configure(EntityTypeBuilder<UserPermissionDeclaration> builder)
        {
            builder.HasOne(up => up.User)
                .WithMany(u => u.PermissionDeclarations)
                .HasForeignKey(up => up.UserId);

            builder.ToTable("UserPermissionDeclarations");
        }
    }

    public class OrganizationPermissionDeclarationConfig : IEntityTypeConfiguration<OrganizationPermissionDeclaration>
    {
        public void Configure(EntityTypeBuilder<OrganizationPermissionDeclaration> builder)
        {
            builder.HasOne(op => op.Organization)
                .WithMany(o => o.PermissionDeclarations)
                .HasForeignKey(op => op.OrganizationId);

            builder.HasOne(op => op.ParentPermissionDeclaration)
                .WithMany()
                .HasForeignKey(op => op.ParentPermissionDeclarationId);

            builder.ToTable("OrganizationPermissionDeclarations");
        }
    }

    public class RequestHandlerPermissionDeclarationConfig : IEntityTypeConfiguration<RequestHandlerPermissionDeclaration>
    {
        public void Configure(EntityTypeBuilder<RequestHandlerPermissionDeclaration> builder)
        {
            builder.HasOne(rhp => rhp.Rule)
                .WithMany(r => r.PermissionDeclarations)
                .HasForeignKey(rhp => rhp.RuleId);

            builder.ToTable("RequestHandlerPermissionDeclarations");
        }
    }

    public class RequestAuthorizationRuleConfig : IEntityTypeConfiguration<RequestAuthorizationRule>
    {
        public void Configure(EntityTypeBuilder<RequestAuthorizationRule> builder)
        {
            builder.ToTable("RequestAuthorizationRules");
        }
    }

    public class RequestHandlerPermissionDeclarationRoleConfig : IEntityTypeConfiguration<RequestHandlerPermissionDeclarationRole>
    {
        public void Configure(EntityTypeBuilder<RequestHandlerPermissionDeclarationRole> builder)
        {
            builder.HasOne(rhpr => rhpr.Role)
                .WithMany()
                .HasForeignKey(rhpr => rhpr.RoleId);

            builder.HasOne(rhpr => rhpr.PermissionDeclaration)
                .WithMany(rhp => rhp.PermissionDeclarationRoles)
                .HasForeignKey(rhpr => rhpr.PermissionDeclarationId);

            builder.ToTable("RequestHandlerPermissionDeclarationRoles");
        }
    }

    public class RequestHandlerPermissionDeclarationOrganizationConfig : IEntityTypeConfiguration<RequestHandlerPermissionDeclarationOrganization>
    {
        public void Configure(EntityTypeBuilder<RequestHandlerPermissionDeclarationOrganization> builder)
        {
            builder.HasOne(rhpo => rhpo.Organization)
                .WithMany()
                .HasForeignKey(rhpo => rhpo.OrganizationId);

            builder.HasOne(rhpo => rhpo.PermissionDeclaration)
                .WithMany(rhp => rhp.PermissionDeclarationOrganizations)
                .HasForeignKey(rhpo => rhpo.PermissionDeclarationId);

            builder.ToTable("RequestHandlerPermissionDeclarationOrganizations");
        }
    }
}
