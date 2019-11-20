using System;
using System.Collections.Generic;
using CoreDX.Application.EntityFrameworkCore.Extensions;
using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreDX.Application.EntityFrameworkCore.EntityConfiguration
{
    /// <summary>
    /// 身份实体配置
    /// </summary>
    public static class ApplicationIdentityEntityConfiguration
    {
        public static void ConfigUser<TKey, TUser, TRole, TUserRole, TUserClaim, TUserLogin, TUserToken, TUserOrganization, TOrganization>(
            this EntityTypeBuilder<TUser> builder)
            where TKey : struct, IEquatable<TKey>
            where TUser : ApplicationUser<TKey, TUser, TRole, TUserRole, TUserClaim, TUserLogin, TUserToken, TUserOrganization, TOrganization>
            where TRole : class, IEntity<TKey>
            where TUserRole : ApplicationUserRole<TKey, TUser, TRole>
            where TUserClaim : ApplicationUserClaim<TKey, TUser>
            where TUserLogin : ApplicationUserLogin<TKey, TUser>
            where TUserToken : ApplicationUserToken<TKey, TUser>
            where TUserOrganization : ApplicationUserOrganization<TKey, TUser, TOrganization, TUserOrganization>
            where TOrganization : Organization<TKey, TOrganization, TUser, TUserOrganization>
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

        public static void ConfigRole<TKey, TUser, TRole, TUserRole, TRoleClaim>(
            this EntityTypeBuilder<TRole> builder)
            where TKey : struct, IEquatable<TKey>
            where TUser : class, IEntity<TKey>
            where TRole : ApplicationRole<TKey, TUser, TRole, TUserRole, TRoleClaim>
            where TUserRole : ApplicationUserRole<TKey, TUser, TRole>
            where TRoleClaim : ApplicationRoleClaim<TKey, TUser, TRole>
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

        public static void ConfigUserClaim<TUserClaim, TUser, TRole, TOrganization, TUserOrganization, TKey>(
            this EntityTypeBuilder<TUserClaim> builder)
            where TKey : struct, IEquatable<TKey>
            where TUserClaim : ApplicationUserClaim<TKey, TUser>
            where TUser : class, IEntity<TKey>
            where TRole : IEntity<TKey>
            where TOrganization : Organization<TKey, TOrganization, TUser, TUserOrganization>
            where TUserOrganization : ApplicationUserOrganization<TKey, TUser, TOrganization, TUserOrganization>
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

        public static void ConfigOrganization<TOrganization, TUser, TUserOrganization, TKey>(
            this EntityTypeBuilder<TOrganization> builder)
            where TOrganization : Organization<TKey, TOrganization, TUser, TUserOrganization>
            where TKey : struct, IEquatable<TKey>
            where TUser : class, IEntity<TKey>
            where TUserOrganization : ApplicationUserOrganization<TKey, TUser, TOrganization, TUserOrganization>
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
            where TUserOrganization : ApplicationUserOrganization<TKey, TUser, TOrganization, TUserOrganization>
            where TUser : class, IEntity<TKey>
            where TOrganization : Organization<TKey, TOrganization, TUser, TUserOrganization>
        {
            builder.ConfigForManyToManyReferenceEntityBase<TUserOrganization, TKey, TUser>();
            builder.HasKey(e => new { e.UserId, e.OrganizationId });
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
            builder.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId);
            builder.ToTable("AppUserOrganizations");
        }

        public static void ConfigRoleView<ApplicationRoleView>(this EntityTypeBuilder<ApplicationRoleView> builder)
            where ApplicationRoleView : Domain.Entity.Identity.ApplicationRoleView
        {
            //builder.HasOne(e => e.Parent)
            //    .WithMany(e => (IEnumerable<ApplicationRoleView>)e.Children)
            //    .HasForeignKey(e => e.ParentId);
            builder.ToView("view_tree_AppRoles");
        }
    }
}
