using System;
using CoreDX.Application.EntityFrameworkCore.EntityConfiguration;
using CoreDX.Domain.Entity.Permission;
using CoreDX.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CoreDX.Application.EntityFrameworkCore
{
    /// <summary>
    /// 权限数据上下文
    /// </summary>
    public class ApplicationPermissionDbContext : ApplicationPermissionDbContext<int, int, PermissionDefinition, UserPermissionDeclaration, RolePermissionDeclaration, OrganizationPermissionDeclaration, RequestAuthorizationRule, AuthorizationRule>
    {
        public ApplicationPermissionDbContext(DbContextOptions<ApplicationPermissionDbContext> options)
            : base(options)
        { }

        public ApplicationPermissionDbContext() { }
    }

    /// <summary>
    /// 权限数据上下文
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TIdentityKey">身份主键类型</typeparam>
    /// <typeparam name="TPermissionDefinition">权限定义</typeparam>
    /// <typeparam name="TUserPermissionDeclaration">用户权限声明</typeparam>
    /// <typeparam name="TRolePermissionDeclaration">角色权限声明</typeparam>
    /// <typeparam name="TOrganizationPermissionDeclaration">组织权限声明</typeparam>
    /// <typeparam name="TRequestAuthorizationRule">请求授权规则</typeparam>
    /// <typeparam name="TAuthorizationRule">授权规则</typeparam>
    public class ApplicationPermissionDbContext<TKey, TIdentityKey, TPermissionDefinition, TUserPermissionDeclaration, TRolePermissionDeclaration, TOrganizationPermissionDeclaration, TRequestAuthorizationRule, TAuthorizationRule>
        : DbContext
        where TKey : struct, IEquatable<TKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TPermissionDefinition : PermissionDefinition<TKey, TIdentityKey>
        where TUserPermissionDeclaration : UserPermissionDeclaration<TKey, TIdentityKey, TPermissionDefinition>
        where TRolePermissionDeclaration : RolePermissionDeclaration<TKey, TIdentityKey, TPermissionDefinition>
        where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey, TIdentityKey, TPermissionDefinition>
        where TRequestAuthorizationRule : RequestAuthorizationRule<TKey, TIdentityKey, TAuthorizationRule, TRequestAuthorizationRule>
        where TAuthorizationRule : AuthorizationRule<TKey, TIdentityKey, TRequestAuthorizationRule, TAuthorizationRule>
    {
        #region DbSet

        public virtual DbSet<TPermissionDefinition> PermissionDefinitions { get; set; }
        public virtual DbSet<TUserPermissionDeclaration> UserPermissionDeclarations { get; set; }
        public virtual DbSet<TRolePermissionDeclaration> RolePermissionDeclarations { get; set; }
        public virtual DbSet<TOrganizationPermissionDeclaration> OrganizationPermissionDeclarations { get; set; }
        public virtual DbSet<TRequestAuthorizationRule> RequestAuthorizationRules { get; set; }
        public virtual DbSet<TAuthorizationRule> AuthorizationRules { get; set; }

        #endregion

        /// <summary>初始化新的实例</summary>
        /// <param name="options">应用于ApplicationIdentityDbContext的选项</param>
        public ApplicationPermissionDbContext(DbContextOptions options)
            : base(options)
        {}

        public ApplicationPermissionDbContext(){}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TPermissionDefinition>().ConfigPermissionDefinition<TPermissionDefinition, TKey, TIdentityKey>();
            builder.Entity<TUserPermissionDeclaration>().ConfigUserPermissionDeclaration<TUserPermissionDeclaration, TKey, TIdentityKey, TPermissionDefinition>();
            builder.Entity<TRolePermissionDeclaration>().ConfigRolePermissionDeclaration<TRolePermissionDeclaration, TKey, TIdentityKey, TPermissionDefinition>();
            builder.Entity<TOrganizationPermissionDeclaration>().ConfigOrganizationPermissionDeclaration<TOrganizationPermissionDeclaration, TKey, TIdentityKey, TPermissionDefinition>();
            builder.Entity<TRequestAuthorizationRule>().ConfigRequestAuthorizationRule<TRequestAuthorizationRule, TKey, TIdentityKey, TAuthorizationRule>();
            builder.Entity<TAuthorizationRule>().ConfigAuthorizationRule<TAuthorizationRule, TKey, TIdentityKey, TRequestAuthorizationRule>();

            builder.ConfigDatabaseDescription();
            //builder.ConfigPropertiesGuidToStringConverter();
        }
    }
}
