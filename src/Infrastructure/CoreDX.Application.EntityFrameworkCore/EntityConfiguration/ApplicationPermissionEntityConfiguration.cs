using System;
using CoreDX.Application.EntityFrameworkCore.Extensions;
using CoreDX.Domain.Entity.Permission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreDX.Application.EntityFrameworkCore.EntityConfiguration
{
    /// <summary>
    /// 权限实体配置
    /// </summary>
    public static class ApplicationPermissionEntityConfiguration
    {
        public static void ConfigPermissionDefinition<TPermissionDefinition, TKey, TIdentityKey>(
            this EntityTypeBuilder<TPermissionDefinition> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TPermissionDefinition : PermissionDefinition<TKey, TIdentityKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TPermissionDefinition>();
            builder.ToTable("PermissionDefinitions");
        }

        public static void ConfigUserPermissionDeclaration<TUserPermissionDeclaration, TKey, TIdentityKey, TPermissionDefinition>(
            this EntityTypeBuilder<TUserPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TUserPermissionDeclaration : UserPermissionDeclaration<TKey, TIdentityKey, TPermissionDefinition>
            where TPermissionDefinition : PermissionDefinition<TKey, TIdentityKey>
        {
            builder.HasKey(e => new { e.UserId, e.PermissionDefinitionId });
            builder.ConfigForIManyToManyReferenceEntity();
            builder.HasOne(e => e.PermissionDefinition)
                .WithMany()
                .HasForeignKey(e => e.PermissionDefinitionId);
            builder.ToTable("UserPermissionDeclarations");
        }

        public static void ConfigRolePermissionDeclaration<TRolePermissionDeclaration, TKey, TIdentityKey, TPermissionDefinition>(
            this EntityTypeBuilder<TRolePermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TRolePermissionDeclaration : RolePermissionDeclaration<TKey, TIdentityKey, TPermissionDefinition>
            where TPermissionDefinition : PermissionDefinition<TKey, TIdentityKey>
        {
            builder.HasKey(e => new {e.RoleId, e.PermissionDefinitionId});
            builder.ConfigForIManyToManyReferenceEntity();
            builder.HasOne(e => e.PermissionDefinition)
                .WithMany()
                .HasForeignKey(e => e.PermissionDefinitionId);
            builder.ToTable("RolePermissionDeclarations");
        }

        public static void ConfigOrganizationPermissionDeclaration<TOrganizationPermissionDeclaration, TKey, TIdentityKey, TPermissionDefinition>(
            this EntityTypeBuilder<TOrganizationPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey, TIdentityKey, TPermissionDefinition>
            where TPermissionDefinition : PermissionDefinition<TKey, TIdentityKey>
        {
            builder.HasKey(e => new {e.OrganizationId, e.PermissionDefinitionId});
            builder.ConfigForIManyToManyReferenceEntity();
            builder.HasOne(e => e.PermissionDefinition)
                .WithMany()
                .HasForeignKey(e => e.PermissionDefinitionId);
            builder.ToTable("OrganizationPermissionDeclarations");
        }

        public static void ConfigRequestAuthorizationRule<TRequestAuthorizationRule, TKey, TIdentityKey, TAuthorizationRule>(
            this EntityTypeBuilder<TRequestAuthorizationRule> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TAuthorizationRule : AuthorizationRule<TKey, TIdentityKey, TRequestAuthorizationRule, TAuthorizationRule>
            where TRequestAuthorizationRule : RequestAuthorizationRule<TKey, TIdentityKey, TAuthorizationRule, TRequestAuthorizationRule>
        {
            builder.ConfigForDomainEntityBase<TKey, TRequestAuthorizationRule, TIdentityKey>();
            builder.HasOne(e => e.AuthorizationRule)
                .WithMany()
                .HasForeignKey(e => e.AuthorizationRuleId);
            builder.ToTable("AppRequestAuthorizationRules");
        }

        public static void ConfigAuthorizationRule<TAuthorizationRule, TKey, TIdentityKey, TRequestAuthorizationRule>(
            this EntityTypeBuilder<TAuthorizationRule> builder)
            where TAuthorizationRule : AuthorizationRule<TKey, TIdentityKey, TRequestAuthorizationRule, TAuthorizationRule>
            where TRequestAuthorizationRule : RequestAuthorizationRule<TKey, TIdentityKey, TAuthorizationRule, TRequestAuthorizationRule>
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TAuthorizationRule, TIdentityKey>();
            builder.HasMany(e => e.RequestAuthorizationRules)
                .WithOne()
                .HasForeignKey(e => e.AuthorizationRuleId);
            builder.ToTable("AppAuthorizationRules");
        }
    }
}
