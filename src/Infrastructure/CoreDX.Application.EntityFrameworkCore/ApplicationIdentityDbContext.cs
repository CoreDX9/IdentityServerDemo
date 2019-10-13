using System;
using CoreDX.Application.EntityFrameworkCore.EntityConfiguration;
using CoreDX.Domain.Model.Entity.Identity;
using CoreDX.Domain.Model.Entity.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreDX.Application.EntityFrameworkCore
{
    public class ApplicationIdentityDbContext : ApplicationIdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken, Organization, PermissionDefinition, ApplicationUserOrganization, UserPermissionDeclaration, RolePermissionDeclaration, OrganizationPermissionDeclaration, RequestAuthorizationRule, AuthorizationRule> { }


    public class ApplicationIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TOrganization, TPermissionDefinition, TUserOrganization, TUserPermissionDeclaration, TRolePermissionDeclaration, TOrganizationPermissionDeclaration, TRequestAuthorizationRule, TAuthorizationRule>
        : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : ApplicationUser<TKey, TUser, TRole, TOrganization>
        where TRole : ApplicationRole<TKey, TUser, TRole>
        where TKey : struct, IEquatable<TKey>
        where TUserClaim : ApplicationUserClaim<TKey, TUser>
        where TUserRole : ApplicationUserRole<TKey, TUser, TRole>
        where TUserLogin : ApplicationUserLogin<TKey, TUser>
        where TRoleClaim : ApplicationRoleClaim<TKey, TUser, TRole>
        where TUserToken : ApplicationUserToken<TKey, TUser>
        where TOrganization : Organization<TKey, TOrganization, TUser>
        where TPermissionDefinition : PermissionDefinition<TKey, TKey>
        where TUserOrganization : ApplicationUserOrganization<TKey, TUser, TOrganization>
        where TUserPermissionDeclaration : UserPermissionDeclaration<TKey, TKey>
        where TRolePermissionDeclaration : RolePermissionDeclaration<TKey, TKey>
        where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey, TKey>
        where TRequestAuthorizationRule : RequestAuthorizationRule<TKey, TKey>
        where TAuthorizationRule : AuthorizationRule<TKey, TKey>
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

        public virtual DbQuery<ApplicationRoleView> IdentityRoleView { get; set; }

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

            builder.Entity<TUser>().ConfigUser<TUser, TRole, TOrganization, TKey>();
            builder.Entity<TRole>().ConfigRole<TUser, TRole, TKey>();
            builder.Entity<TUserClaim>().ConfigUserClaim<TUserClaim, TUser, TRole, TOrganization, TKey>();
            builder.Entity<TUserRole>().ConfigUserRole<TUserRole, TUser, TRole, TKey>();
            builder.Entity<TUserLogin>().ConfigTUserLogin<TUserLogin, TUser, TKey>();
            builder.Entity<TRoleClaim>().ConfigRoleClaim<TRoleClaim, TUser, TRole, TKey>();
            builder.Entity<TUserToken>().ConfigTUserToken<TUserToken, TUser, TKey>();
            builder.Entity<TOrganization>().ConfigOrganization<TOrganization, TUser, TKey>();
            builder.Entity<TPermissionDefinition>().ConfigPermissionDefinition<TPermissionDefinition, TUser, TKey>();
            builder.Entity<TUserOrganization>().ConfigUserOrganization<TUserOrganization, TUser, TOrganization, TKey>();
            builder.Entity<TUserPermissionDeclaration>().ConfigUserPermissionDeclaration<TUserPermissionDeclaration, TUser, TRole, TOrganization, TKey>();
            builder.Entity<TRolePermissionDeclaration>().ConfigRolePermissionDeclaration<TRolePermissionDeclaration, TUser, TRole, TKey>();
            builder.Entity<TOrganizationPermissionDeclaration>().ConfigOrganizationPermissionDeclaration<TOrganizationPermissionDeclaration, TOrganization, TUser, TKey>();
            builder.Entity<TRequestAuthorizationRule>().ConfigRequestAuthorizationRule<TRequestAuthorizationRule, TUser, TKey>();
            builder.Entity<TAuthorizationRule>().ConfigAuthorizationRule<TAuthorizationRule, TUser, TKey>();
        }
    }
}
