using System;
using System.Collections.Generic;
using CoreDX.Application.Domain.Model.Entity.Core;
using CoreDX.Application.Domain.Model.Entity.Identity;
using CoreDX.Application.Domain.Model.Entity.Security;
using CoreDX.Infrastructure.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreDX.Infrastructure.EntityFrameworkCore.EntityConfiguration
{
    public static class ApplicationDomainEntityConfiguration
    {
        public static void ConfigPermissionDefinition<TPermissionDefinition, TIdentityUser, TKey>(
            this EntityTypeBuilder<TPermissionDefinition> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityUser : class, IEntity<TKey>
            where TPermissionDefinition : PermissionDefinition<TKey, TIdentityUser>
        {
            builder.ConfigForDomainEntityBase<TKey, TPermissionDefinition, TIdentityUser, TKey>();
            builder.ToTable("PermissionDefinitions");
        }

        public static void ConfigUserPermissionDeclaration<TUserPermissionDeclaration, TIdentityUser, TIdentityRole, TOrganization, TKey>(
            this EntityTypeBuilder<TUserPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityUser : ApplicationUser<TKey, TIdentityUser, TIdentityRole, TOrganization>
            where TOrganization : Organization<TKey, TOrganization, TIdentityUser>
            where TIdentityRole : IEntity<TKey>
            where TUserPermissionDeclaration : UserPermissionDeclaration<TKey, TIdentityUser>
        {
            builder.HasKey(e => new { e.UserId, e.PermissionDefinitionId });
            builder.ConfigForIManyToManyReferenceEntity<TUserPermissionDeclaration, TKey, TIdentityUser>();
            builder.ConfigForILastModifierRecordable<TUserPermissionDeclaration, TIdentityUser, TKey>();
            builder.HasOne(up => up.User)
                .WithMany(u => (IEnumerable<TUserPermissionDeclaration>)u.PermissionDeclarations)
                .HasForeignKey(up => up.UserId);
            builder.HasOne(e => e.PermissionDefinition)
                .WithMany()
                .HasForeignKey(e => e.PermissionDefinitionId);
            builder.ToTable("UserPermissionDeclarations");
        }

        public static void ConfigRolePermissionDeclaration<TRolePermissionDeclaration, TIdentityUser, TIdentityRole, TKey>(
            this EntityTypeBuilder<TRolePermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityUser : class, IEntity<TKey>
            where TIdentityRole : ApplicationRole<TKey, TIdentityUser, TIdentityRole>
            where TRolePermissionDeclaration : RolePermissionDeclaration<TKey, TIdentityUser, TIdentityRole>
        {
            builder.HasKey(e => new {e.RoleId, e.PermissionDefinitionId});
            builder.ConfigForIManyToManyReferenceEntity<TRolePermissionDeclaration, TKey, TIdentityUser>();
            builder.ConfigForILastModifierRecordable<TRolePermissionDeclaration, TIdentityUser, TKey>();
            builder.HasOne(rp => rp.Role)
                .WithMany(r => (IEnumerable<TRolePermissionDeclaration>)r.PermissionDeclarations)
                .HasForeignKey(rp => rp.RoleId);
            builder.HasOne(e => e.PermissionDefinition)
                .WithMany()
                .HasForeignKey(e => e.PermissionDefinitionId);
            builder.ToTable("RolePermissionDeclarations");
        }

        public static void ConfigOrganizationPermissionDeclaration<TOrganizationPermissionDeclaration, TOrganization, TIdentityUser, TKey>(
            this EntityTypeBuilder<TOrganizationPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TOrganization : Organization<TKey, TOrganization, TIdentityUser>
            where TIdentityUser : class, IEntity<TKey>
            where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey, TOrganization, TIdentityUser>
        {
            builder.HasKey(e => new {e.OrganizationId, e.PermissionDefinitionId});
            builder.ConfigForIManyToManyReferenceEntity<TOrganizationPermissionDeclaration, TKey, TIdentityUser>();
            builder.ConfigForILastModifierRecordable<TOrganizationPermissionDeclaration, TIdentityUser, TKey>();
            builder.HasOne(op => op.Organization)
                .WithMany(o => (IEnumerable<TOrganizationPermissionDeclaration>)o.PermissionDeclarations)
                .HasForeignKey(op => op.OrganizationId);
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
        }

        public static void ConfigRoleClaim<TRoleClaim, TUser, TRole, TKey>(
            this EntityTypeBuilder<TRoleClaim> builder)
            where TKey : struct, IEquatable<TKey>
            where TRoleClaim : ApplicationRoleClaim<TKey, TUser, TRole>
            where TUser : class, IEntity<TKey>
            where TRole : IEntity<TKey>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigForICreatorRecordable<TRoleClaim, TUser, TKey>();
            builder.ConfigForILastModifierRecordable<TRoleClaim, TUser, TKey>();
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
        }

        public static void ConfigOrganization<TOrganization, TUser, TKey>(
            this EntityTypeBuilder<TOrganization> builder)
            where TOrganization : Organization<TKey, TOrganization, TUser>
            where TKey : struct, IEquatable<TKey>
            where TUser : class, IEntity<TKey>
        {
            builder.ConfigForDomainTreeEntityBase<TKey, TOrganization, TKey, TUser>();
            builder.HasMany(e => e.PermissionDeclarations)
                .WithOne(e => e.Organization)
                .HasForeignKey(e => e.OrganizationId);
            builder.HasMany(e => e.UserOrganizations)
                .WithOne(e => e.Organization)
                .HasForeignKey(e => e.OrganizationId);
            builder.ToTable("Organizations");
        }
    }
}
