using System;
using CoreDX.Application.EntityFrameworkCore.EntityConfiguration;
using CoreDX.Domain.Entity.Identity;
using CoreDX.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreDX.Application.EntityFrameworkCore
{
    /// <summary>
    /// 身份数据上下文
    /// </summary>
    public class ApplicationIdentityDbContext : ApplicationIdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken, Organization, ApplicationUserOrganization> { }

    /// <summary>
    /// 身份数据上下文
    /// </summary>
    /// <typeparam name="TUser">用户</typeparam>
    /// <typeparam name="TRole">角色</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TUserClaim">用户宣称</typeparam>
    /// <typeparam name="TUserRole">用户角色</typeparam>
    /// <typeparam name="TUserLogin">用户登录数据</typeparam>
    /// <typeparam name="TRoleClaim">角色宣称</typeparam>
    /// <typeparam name="TUserToken">用户令牌</typeparam>
    /// <typeparam name="TOrganization">组织</typeparam>
    /// <typeparam name="TUserOrganization">用户组织</typeparam>
    public class ApplicationIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            TUserToken, TOrganization, TUserOrganization>
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
        where TUserOrganization : ApplicationUserOrganization<TKey, TUser, TOrganization>
    {

        #region DbSet

        public virtual DbSet<TOrganization> Organizations { get; set; }
        public virtual DbSet<TUserOrganization> UserOrganizations { get; set; }

        #endregion

        #region DbQuery要改成ef core 3.x 的 Entity视图

        public virtual DbQuery<ApplicationRoleView> IdentityRoleView { get; set; }

        #endregion

        /// <summary>初始化新的实例</summary>
        /// <param name="options">应用于ApplicationIdentityDbContext的选项</param>
        public ApplicationIdentityDbContext(
            DbContextOptions<ApplicationIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin,
                TRoleClaim, TUserToken, TOrganization, TUserOrganization>> options)
            : base(options)
        {
        }

        public ApplicationIdentityDbContext()
        {
        }

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
            builder.Entity<TUserOrganization>().ConfigUserOrganization<TUserOrganization, TUser, TOrganization, TKey>();

            builder.ConfigKeyGuidToStringConverter();
        }
    }
}
