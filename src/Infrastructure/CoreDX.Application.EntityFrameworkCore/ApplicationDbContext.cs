using CoreDX.Domain.Entity.App.Management;
using CoreDX.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using CoreDX.Domain.Entity.App.Sample;
using CoreDX.Application.EntityFrameworkCore.Extensions;

namespace CoreDX.Application.EntityFrameworkCore
{
    /// <summary>
    /// 应用数据上下文
    /// </summary>
    public class ApplicationDbContext : DbContext
    {

        #region DbSet

        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<Domain.Entity.App.Sample.Domain> Domains { get; set; }
        public virtual DbSet<TreeDomain> TreeDomains { get; set; }

        #endregion

        #region DbSet(Views)

        public virtual DbSet<TreeDomainView> TreeDomainViews { get; set; }
        public virtual DbSet<MenuView> MenuViews { get; set; }
        #endregion

        /// <summary>初始化新的实例</summary>
        /// <param name="options">应用于ApplicationIdentityDbContext的选项</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}

        public ApplicationDbContext(){}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Menu>(b =>
                {
                    b.ConfigForDomainTreeEntityBase<int, Menu, int>();
                    b.OwnsOne(oe => oe.MenuIcon);
                });
            builder.Entity<MenuView>(b =>
            {
                b.HasOne(e => e.Parent)
                    .WithMany(e => e.Children)
                    .HasForeignKey(e => e.ParentId);
                //b.OwnsOne(oe => oe.MenuIcon);
                b.ToView($"view_tree_Menus");
            });
            builder.Entity<MenuItem>(e => e.OwnsOne(oe => oe.MenuItemIcon));
            builder.Entity<Domain.Entity.App.Sample.Domain>(e => e.OwnsOne(oe => oe.ComplexProperty).OwnsOne(oe => oe.ComplexProperty2));
            builder.Entity<TreeDomain>(b =>
                {
                    b.ConfigForDomainTreeEntityBase<int, TreeDomain, int>();
                });
            builder.Entity<TreeDomainView>(b =>
                {
                    b.HasOne(e => e.Parent)
                        .WithMany(e => e.Children)
                        .HasForeignKey(e => e.ParentId);
                    b.ToView($"view_treeTreeDomains");
                });

            builder.ConfigDatabaseDescription();
            //builder.ConfigPropertiesGuidToStringConverter();
        }
    }
}
