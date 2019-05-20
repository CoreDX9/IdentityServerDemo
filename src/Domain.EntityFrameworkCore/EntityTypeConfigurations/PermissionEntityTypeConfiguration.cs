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

            builder.ToTable("OrganizationPermissionDeclarations");
        }
    }

    public class AuthorizationRuleConfig : IEntityTypeConfiguration<AuthorizationRule>
    {
        public void Configure(EntityTypeBuilder<AuthorizationRule> builder)
        {
            builder.ToTable("AuthorizationRules");
        }
    }

    public class RequestAuthorizationRuleConfig : IEntityTypeConfiguration<RequestAuthorizationRule>
    {
        public void Configure(EntityTypeBuilder<RequestAuthorizationRule> builder)
        {
            builder.HasOne(r => r.AuthorizationRule)
                .WithMany(a => a.RequestAuthorizationRules)
                .HasForeignKey(r => r.AuthorizationRuleId);

            builder.ToTable("RequestAuthorizationRules");
        }
    }
}
