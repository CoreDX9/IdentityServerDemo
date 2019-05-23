using System;
using System.Collections.Generic;
using CoreDX.Application.Domain.Model.Entity.Identity;
using CoreDX.Application.Domain.Model.Entity.Security;
using CoreDX.Infrastructure.EntityFrameworkCore.EntityConfiguration;
using CoreDX.Infrastructure.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreDX.Infrastructure.EntityFrameworkCore
{
    public class ApplicationIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TOrganization, TPermissionDefinition, TUserOrganization, TUserPermissionDeclaration, TRolePermissionDeclaration, TOrganizationPermissionDeclaration, TRequestAuthorizationRule, TAuthorizationRule>
        : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : ApplicationUser<TKey>
        where TRole : ApplicationRole<TKey>
        where TKey : struct, IEquatable<TKey>
        where TUserClaim : ApplicationUserClaim<TKey>
        where TUserRole : ApplicationUserRole<TKey>
        where TUserLogin : ApplicationUserLogin<TKey>
        where TRoleClaim : ApplicationRoleClaim<TKey>
        where TUserToken : ApplicationUserToken<TKey>
        where TOrganization : Organization<TKey>
        where TPermissionDefinition : PermissionDefinition<TKey>
        where TUserOrganization : ApplicationUserOrganization<TKey>
        where TUserPermissionDeclaration : UserPermissionDeclaration<TKey>
        where TRolePermissionDeclaration : RolePermissionDeclaration<TKey>
        where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey>
        where TRequestAuthorizationRule : RequestAuthorizationRule<TKey>
        where TAuthorizationRule : AuthorizationRule<TKey>
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

            builder.Entity<TPermissionDefinition>().ConfigPermissionDefinition<TPermissionDefinition, TUser, TKey>();
            builder.Entity<TUserPermissionDeclaration>().ConfigUserPermissionDeclaration<TUserPermissionDeclaration, TUser, TKey>();
            builder.Entity<TRolePermissionDeclaration>().ConfigRolePermissionDeclaration<TRolePermissionDeclaration, TUser, TKey>();
            builder.Entity<TOrganizationPermissionDeclaration>().ConfigOrganizationPermissionDeclaration<TOrganizationPermissionDeclaration, TUser, TKey>();

            builder.Entity<TRequestAuthorizationRule>()
                .HasOne(r => r.AuthorizationRule)
                .WithMany(a => (IEnumerable<TRequestAuthorizationRule>)a.RequestAuthorizationRules)
                .HasForeignKey(r => r.AuthorizationRuleId);
            builder.Entity<TRequestAuthorizationRule>().ToTable("RequestAuthorizationRules");

            builder.Entity<TAuthorizationRule>().ToTable("AuthorizationRules");

            var tUserBuilder = builder.Entity<TUser>();
            tUserBuilder.ConfigCreator<TUser, TUser, TKey>();
            tUserBuilder.ConfigLastModifier<TUser, TUser, TKey>();

            builder.Entity<TRole>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            builder.Entity<TRole>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);

            builder.Entity<TUserClaim>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            builder.Entity<TUserClaim>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);

            builder.Entity<TUserRole>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            builder.Entity<TUserRole>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId); 

            builder.Entity<TUserLogin>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
            builder.Entity<TUserLogin>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);

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
            builder.Entity<TUserToken>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);

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
            builder.Entity<TUserOrganization>()
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);

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
