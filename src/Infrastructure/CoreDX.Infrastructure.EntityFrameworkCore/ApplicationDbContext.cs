using System;
using System.Collections.Generic;
using CoreDX.Application.Domain.Model.Entity.Identity;
using CoreDX.Application.Domain.Model.Entity.Security;
using CoreDX.Infrastructure.EntityFrameworkCore.EntityConfiguration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreDX.Infrastructure.EntityFrameworkCore
{
    public class ApplicationIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TOrganization, TPermissionDefinition, TUserOrganization, TUserPermissionDeclaration, TRolePermissionDeclaration, TOrganizationPermissionDeclaration, TRequestAuthorizationRule, TAuthorizationRule>
        : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : ApplicationUser<TKey, TUser, TRole>
        where TRole : ApplicationRole<TKey, TUser, TRole>
        where TKey : struct, IEquatable<TKey>
        where TUserClaim : ApplicationUserClaim<TKey, TUser>
        where TUserRole : ApplicationUserRole<TKey, TUser, TRole>
        where TUserLogin : ApplicationUserLogin<TKey, TUser>
        where TRoleClaim : ApplicationRoleClaim<TKey, TUser, TRole>
        where TUserToken : ApplicationUserToken<TKey, TUser>
        where TOrganization : Organization<TKey, TUser>
        where TPermissionDefinition : PermissionDefinition<TKey, TUser>
        where TUserOrganization : ApplicationUserOrganization<TKey, TUser>
        where TUserPermissionDeclaration : UserPermissionDeclaration<TKey, TUser>
        where TRolePermissionDeclaration : RolePermissionDeclaration<TKey, TUser, TRole>
        where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey, TUser>
        where TRequestAuthorizationRule : RequestAuthorizationRule<TKey, TUser>
        where TAuthorizationRule : AuthorizationRule<TKey, TUser>
    {

        #region DbSet

        public virtual DbSet<TOrganization> Organizations { get; set; }
        public virtual DbSet<TPermissionDefinition> PermissionDefinitions { get; set; }
        public virtual DbSet<TUserOrganization> UserOrganizations { get; set; }
        public virtual DbSet<TUserPermissionDeclaration> UserPermissionDeclarations { get; set; }
        public virtual DbSet<TRolePermissionDeclaration> RolePermissionDeclarations { get; set; }
        public virtual DbSet<TOrganizationPermissionDeclaration> OrganizationPermissionDeclarations { get; set; }
        public virtual DbSet<TRequestAuthorizationRule> RequestAuthorizationRules { get; set; }
        public virtual DbSet<TAuthorizationRule> AuthorizationRules { get; set; }

        #endregion

        #region DbQuery

        //public virtual DbQuery<ApplicationRoleView> IdentityRoleView { get; set; }

        #endregion

        /// <summary>初始化新的实例</summary>
        /// <param name="options">应用于ApplicationIdentityDbContext的选项</param>
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TOrganization, TPermissionDefinition, TUserOrganization, TUserPermissionDeclaration, TRolePermissionDeclaration, TOrganizationPermissionDeclaration, TRequestAuthorizationRule, TAuthorizationRule>> options)
            : base(options)
        {}

        public ApplicationIdentityDbContext(){}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TUser>().ConfigUser<TUser, TRole, TKey>();
            builder.Entity<TRole>().ConfigRole<TUser, TRole, TKey>();
            builder.Entity<TPermissionDefinition>().ConfigPermissionDefinition<TPermissionDefinition, TUser, TKey>();
            builder.Entity<TUserPermissionDeclaration>().ConfigUserPermissionDeclaration<TUserPermissionDeclaration, TUser, TRole, TKey>();
            builder.Entity<TRolePermissionDeclaration>().ConfigRolePermissionDeclaration<TRolePermissionDeclaration, TUser, TRole, TKey>();
            builder.Entity<TOrganizationPermissionDeclaration>().ConfigOrganizationPermissionDeclaration<TOrganizationPermissionDeclaration, TUser, TKey>();
            builder.Entity<TUserClaim>().ConfigUserClaim<TUserClaim, TUser, TRole, TKey>();
            builder.Entity<TUserRole>().ConfigUserRole<TUserRole, TUser, TRole, TKey>();

            builder.Entity<TRequestAuthorizationRule>()
                .HasOne(r => r.AuthorizationRule)
                .WithMany(a => (IEnumerable<TRequestAuthorizationRule>)a.RequestAuthorizationRules)
                .HasForeignKey(r => r.AuthorizationRuleId);
            builder.Entity<TRequestAuthorizationRule>().ToTable("RequestAuthorizationRules");

            builder.Entity<TAuthorizationRule>().ToTable("AuthorizationRules");

            builder.Entity<TUserRole>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            //builder.Entity<TUserRole>()
            //    .HasOne(e => e.LastModifier)
            //    .WithMany()
            //    .HasForeignKey(e => e.LastModifierId); 

            //builder.Entity<TUserLogin>()
            //    .HasOne(e => e.Creator)
            //    .WithMany()
            //    .HasForeignKey(e => e.CreatorId);
            //builder.Entity<TUserLogin>()
            //    .HasOne(e => e.LastModifier)
            //    .WithMany()
            //    .HasForeignKey(e => e.LastModifierId);

            builder.Entity<TRoleClaim>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            builder.Entity<TRoleClaim>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);

            builder.Entity<TUserToken>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            //builder.Entity<TUserToken>()
            //    .HasOne(e => e.LastModifier)
            //    .WithMany()
            //    .HasForeignKey(e => e.LastModifierId);

            builder.Entity<TOrganization>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            builder.Entity<TOrganization>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);

            builder.Entity<TUserOrganization>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            //builder.Entity<TUserOrganization>()
            //    .HasOne(e => e.LastModifier)
            //    .WithMany()
            //    .HasForeignKey(e => e.LastModifierId);

            builder.Entity<TRequestAuthorizationRule>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            builder.Entity<TRequestAuthorizationRule>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);

            builder.Entity<TAuthorizationRule>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            builder.Entity<TAuthorizationRule>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);
        }
    }
}
