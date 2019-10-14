using System;
using CoreDX.Application.EntityFrameworkCore.Extensions;
using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Entity.Identity;
using CoreDX.Domain.Entity.Permission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreDX.Application.EntityFrameworkCore.EntityConfiguration
{
    /// <summary>
    /// 领域实体配置
    /// </summary>
    public static class ApplicationDomainEntityConfiguration
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

        public static void ConfigUserPermissionDeclaration<TUserPermissionDeclaration, TKey, TIdentityKey>(
            this EntityTypeBuilder<TUserPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TUserPermissionDeclaration : UserPermissionDeclaration<TKey, TIdentityKey>
        {
            builder.HasKey(e => new { e.UserId, e.PermissionDefinitionId });
            builder.ConfigForIManyToManyReferenceEntity();
            builder.HasOne(e => e.PermissionDefinition)
                .WithMany()
                .HasForeignKey(e => e.PermissionDefinitionId);
            builder.ToTable("UserPermissionDeclarations");
        }

        public static void ConfigRolePermissionDeclaration<TRolePermissionDeclaration, TKey, TIdentityKey>(
            this EntityTypeBuilder<TRolePermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TRolePermissionDeclaration : RolePermissionDeclaration<TKey, TIdentityKey>
        {
            builder.HasKey(e => new {e.RoleId, e.PermissionDefinitionId});
            builder.ConfigForIManyToManyReferenceEntity();
            builder.HasOne(e => e.PermissionDefinition)
                .WithMany()
                .HasForeignKey(e => e.PermissionDefinitionId);
            builder.ToTable("RolePermissionDeclarations");
        }

        public static void ConfigOrganizationPermissionDeclaration<TOrganizationPermissionDeclaration, TKey, TIdentityKey>(
            this EntityTypeBuilder<TOrganizationPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey, TIdentityKey>
        {
            builder.HasKey(e => new {e.OrganizationId, e.PermissionDefinitionId});
            builder.ConfigForIManyToManyReferenceEntity();
            builder.HasOne(e => e.PermissionDefinition)
                .WithMany()
                .HasForeignKey(e => e.PermissionDefinitionId);
            builder.ToTable("OrganizationPermissionDeclarations");
        }

        public static void ConfigUser<TUser, TRole, TOrganization, TKey>(
            this EntityTypeBuilder<TUser> builder)
            where TKey : struct, IEquatable<TKey>
            where TRole : class, IEntity<TKey>
            where TOrganization : Organization<TKey, TOrganization, TUser>
            where TUser : ApplicationUser<TKey, TUser, TRole, TOrganization>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigForIOptimisticConcurrencySupported();
            builder.ConfigForICreatorRecordable<TUser, TUser, TKey>();
            builder.ConfigForILastModifierRecordable<TUser, TUser, TKey>();
            builder.HasMany(e => e.UserOrganizations)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            // Each User can have many UserClaims
            builder.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
            // Each User can have many UserLogins
            builder.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();
            // Each User can have many UserTokens
            builder.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();
            // Each User can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            builder.ToTable("AppUsers");
        }

        public static void ConfigRole<TUser, TRole, TKey>(
            this EntityTypeBuilder<TRole> builder)
            where TKey : struct, IEquatable<TKey>
            where TUser : class, IEntity<TKey>
            where TRole : ApplicationRole<TKey, TUser, TRole>
        {
            builder.ConfigForIDomainTreeEntity<TKey, TRole>();
            builder.ConfigForIOptimisticConcurrencySupported();
            builder.ConfigForICreatorRecordable<TRole, TUser, TKey>();
            builder.ConfigForILastModifierRecordable<TRole, TUser, TKey>();
            // Each Role can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            // Each Role can have many associated RoleClaims
            builder.HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
            builder.ToTable("AppRoles");
        }

        public static void ConfigUserClaim<TUserClaim, TUser, TRole, TOrganization, TKey>(
            this EntityTypeBuilder<TUserClaim> builder)
            where TKey : struct, IEquatable<TKey>
            where TUserClaim : ApplicationUserClaim<TKey, TUser>
            where TUser : ApplicationUser<TKey, TUser, TRole, TOrganization>
            where TRole : IEntity<TKey>
            where TOrganization : Organization<TKey, TOrganization, TUser>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigForICreatorRecordable<TUserClaim, TUser, TKey>();
            builder.ConfigForILastModifierRecordable<TUserClaim, TUser, TKey>();
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
            builder.ToTable("AppUserClaims");
        }

        public static void ConfigUserRole<TUserRole, TUser, TRole, TKey>(
            this EntityTypeBuilder<TUserRole> builder)
            where TKey : struct, IEquatable<TKey>
            where TUserRole : ApplicationUserRole<TKey, TUser, TRole>
            where TUser : class, IEntity<TKey>
            where TRole : class, IEntity<TKey>
        {
            builder.ConfigForIManyToManyReferenceEntity<TUserRole, TKey, TUser>();
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
            builder.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId);
            builder.ToTable("AppUserRoles");
        }

        public static void ConfigTUserLogin<TUserLogin, TUser, TKey>(
            this EntityTypeBuilder<TUserLogin> builder)
        where TKey : struct, IEquatable<TKey>
            where TUserLogin : ApplicationUserLogin<TKey, TUser>
            where TUser : class, IEntity<TKey>
        {
            builder.ConfigForICreatorRecordable<TUserLogin, TUser, TKey>();
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
            builder.ToTable("AppUserLogins");
        }

        public static void ConfigRoleClaim<TRoleClaim, TUser, TRole, TKey>(
            this EntityTypeBuilder<TRoleClaim> builder)
            where TKey : struct, IEquatable<TKey>
            where TRoleClaim : ApplicationRoleClaim<TKey, TUser, TRole>
            where TUser : class, IEntity<TKey>
            where TRole : class, IEntity<TKey>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigForICreatorRecordable<TRoleClaim, TUser, TKey>();
            builder.ConfigForILastModifierRecordable<TRoleClaim, TUser, TKey>();
            builder.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId);
            builder.ToTable("AppRoleClaims");
        }

        public static void ConfigTUserToken<TUserToken, TUser, TKey>(
            this EntityTypeBuilder<TUserToken> builder)
            where TKey : struct, IEquatable<TKey>
            where TUserToken : ApplicationUserToken<TKey, TUser>
            where TUser : class, IEntity<TKey>
        {
            builder.ConfigForICreatorRecordable<TUserToken, TUser, TKey>();
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
            builder.ToTable("AppUserTokens");
        }

        public static void ConfigOrganization<TOrganization, TUser, TKey>(
            this EntityTypeBuilder<TOrganization> builder)
            where TOrganization : Organization<TKey, TOrganization, TUser>
            where TKey : struct, IEquatable<TKey>
            where TUser : class, IEntity<TKey>
        {
            builder.ConfigForDomainTreeEntityBase<TKey, TOrganization, TKey, TUser>();
            builder.HasMany(e => e.UserOrganizations)
                .WithOne(e => e.Organization)
                .HasForeignKey(e => e.OrganizationId);
            builder.ToTable("AppOrganizations");
        }

        public static void ConfigUserOrganization<TUserOrganization, TUser, TOrganization, TKey>(
            this EntityTypeBuilder<TUserOrganization> builder)
            where TKey : struct, IEquatable<TKey>
            where TUserOrganization : ApplicationUserOrganization<TKey, TUser, TOrganization>
            where TUser : class, IEntity<TKey>
            where TOrganization : Organization<TKey, TOrganization, TUser>
        {
            builder.ConfigForManyToManyReferenceEntityBase<TUserOrganization, TKey, TUser>();
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
            builder.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId);
            builder.ToTable("AppUserOrganizations");
        }

        public static void ConfigRequestAuthorizationRule<TRequestAuthorizationRule, TKey, TIdentityKey>(
            this EntityTypeBuilder<TRequestAuthorizationRule> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TRequestAuthorizationRule : RequestAuthorizationRule<TKey, TIdentityKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TRequestAuthorizationRule, TIdentityKey>();
            builder.HasOne(e => e.AuthorizationRule)
                .WithMany()
                .HasForeignKey(e => e.AuthorizationRuleId);
            builder.ToTable("AppRequestAuthorizationRules");
        }

        public static void ConfigAuthorizationRule<TAuthorizationRule, TKey, TIdentityKey>(
            this EntityTypeBuilder<TAuthorizationRule> builder)
            where TAuthorizationRule : AuthorizationRule<TKey, TIdentityKey>
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
