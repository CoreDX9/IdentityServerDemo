using Domain.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Sample;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.EntityFrameworkCore.Extensions;
using Domain.Security;
using EntityFrameworkCore.Extensions.Extensions;
using Repository.RabbitMQ;

namespace Repository.EntityFrameworkCore.Identity
{
    /// <summary>
    /// 基于Identity的数据上下文
    /// 实体历史记录不在Startup注册就不用
    /// </summary>
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>, IEntityHistoryTrackable
    {
        private readonly IEntityHistoryRecorder _entityHistoryRecorder;

        #region DbSet

        public virtual DbSet<Domain.Sample.Domain> Domains { get; set; }
        public virtual DbSet<TreeDomain> TreeDomains { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<PermissionDefinition> PermissionDefinitions { get; set; }
        public virtual DbSet<UserPermissionDeclaration> UserPermissionDeclarations { get; set; }
        public virtual DbSet<RolePermissionDeclaration> RolePermissionDeclarations { get; set; }
        public virtual DbSet<OrganizationPermissionDeclaration> OrganizationPermissionDeclarations { get; set; }
        public virtual DbSet<RequestHandlerPermissionDeclaration> RequestHandlerPermissionDeclarations { get; set; }

        #endregion

        #region DbQuery

        public virtual DbQuery<ApplicationRoleView> IdentityRoleView { get; set; }
        public virtual DbQuery<TreeDomainView> TreeDomainView { get; set; }

        #endregion

        /// <summary>初始化新的实例</summary>
        /// <param name="options">应用于<see cref="ApplicationIdentityDbContext" />的选项</param>
        /// <param name="entityHistoryRecorder">记录实体变更的记录器</param>
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options, IEntityHistoryRecorder entityHistoryRecorder)
            : base(options)
        {
            _entityHistoryRecorder = entityHistoryRecorder;
        }

        /// <summary>初始化新的实例</summary>
        /// <param name="options">应用于<see cref="ApplicationIdentityDbContext" />的选项</param>
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
            : base(options)
        {
        }

        /// <summary>初始化新的实例</summary>
        protected ApplicationIdentityDbContext()
        {
        }

        /// <summary>
        /// 模型配置
        /// </summary>
        /// <param name="builder">
        /// 应用于上下文的模型构造器
        /// </param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //先调用基类方法再增加自己的配置代码，否则基类会覆盖与自己冲突的配置
            base.OnModelCreating(builder);

            //配置Identity主键值转换器、导航属性
            builder.ConfigIdentity()
                //配置权限
                .ConfigPermission()
                //配置唯一索引
                .ConfigUniqueKey()
                //配置数据库表和列说明
                .ConfigDatabaseDescription()
                //配置主键值转换器、自增长标识、默认值、并发标识、导航属性、查询过滤器
                .ConfigEntitiesThatImplementedFromIDomainEntity(this)
                //配置视图名称
                .ConfigViewsThatImplementedFromITree()
                //配置视图查询过滤器
                .ConfigTreeViewsThatSubClassOfDomainTreeEntityViewBase();

            #region 拥有属性（类似ef6.x的复杂属性）配置，实体中某些属性是复杂的类而不是简单数据时的配置，用ToTable()可以把复杂属性内部的简单数据属性存储到指定表，不用则直接把内部的简单数据属性存在主表，内部的其他复杂属性同理

            builder.Entity<Domain.Sample.Domain>(b =>
            {
                b.OwnsOne(c => c.ComplexProperty, cp =>
                {
                    cp.ToTable("ComplexProperty");
                    cp.OwnsOne(c2 => c2.ComplexProperty2, cp2 => cp2.ToTable("ComplexProperty2"));
                });
            });

            #endregion

            #region 一些说明和示例

            //需要在迁移中使用如下代码将OrderNumber列设置为自增长（现在不需要了，留档备份）
            //migrationBuilder.SetOrderNumberToIdentity(string tableName);
            //
            //需要在迁移中使用如下代码为DbQuery<>创建视图（配合DomainTreeEntityViewBase<,,>，如果不是此类的派生需要自己编写sql）
            //反射获取列名并排除导航属性，如果有自定义列名或有其他特殊情况导致列名与属性名不一致的自行解决
            //现在不需要了，可以自动扫描迁移模型，调用migrationBuilder.CreateTreeEntityView(Migration m, Assembly a)即可，留档备份;
            //var columns = typeof(IdentityRole).GetProperties()
            //    .Where(pro =>
            //        !Attribute.IsDefined(pro, typeof(NotMappedAttribute)) &&
            //        new[] { "Parent", "Children", "CreationUser", "LastModificationUser" }.All(str => str != pro.Name))
            //    .Select(pro => pro.Name);
            //使用扩展创建视图，内置配合DomainTreeEntityViewBase<,,>的sql模板，如果有特殊情况自行编写sql
            //migrationBuilder.CreateTreeEntityView("AspNetRoles", columns
            //    //new[]
            //    //{
            //    //    "Id", "Name", "NormalizedName", "ConcurrencyStamp", "ParentId", "RowVersion", "IsDeleted",
            //    //    "CreationTime", "LastModificationTime", "CreationUserId", "LastModificationUserId", "OrderNumber"
            //    //}
            //);
            //
            //在迁移中调用以下扩展方法即可为表和列添加说明
            //在需要添加说明的表实体类和实体属性上添加特性[DbDescription("说明的内容")]
            //migrationBuilder.AddDatabaseDescription(Migration m, Assembly a)

            #endregion
        }

        /// <summary>
        /// 保存实体变更（使用软删除）
        /// </summary>
        /// <returns>受影响的行数</returns>
        public override int SaveChanges()
        {
            return this.SaveChanges(acceptAllChangesOnSuccess:true);
        }

        /// <summary>
        /// 保存实体变更（使用软删除）
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">成功后是否应用变更</param>
        /// <returns>受影响的行数</returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return this.SaveChanges(acceptAllChangesOnSuccess, true);
        }

        /// <summary>
        /// 保存实体变更
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">成功后是否应用变更</param>
        /// <param name="isSoftDelete">是否使用软删除</param>
        /// <returns>受影响的行数</returns>
        public virtual int SaveChanges(bool acceptAllChangesOnSuccess = true, bool isSoftDelete = true)
        {
            this.SetSpecialPropertiesOfEntity();

            var changes = _entityHistoryRecorder?.DiscoveryChanges(this);
           
            if (isSoftDelete)
            {
                this.SetSoftDelete();
            }

            var i = base.SaveChanges(acceptAllChangesOnSuccess);
            _entityHistoryRecorder?.RecordHistory(changes, isSoftDelete);
            return i;
        }

        /// <summary>
        /// 保存实体变更（使用软删除）
        /// </summary>
        /// <param name="cancellationToken">取消记号</param>
        /// <returns>受影响的行数</returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return this.SaveChangesAsync(true, cancellationToken);
        }

        /// <summary>
        /// 保存实体变更（使用软删除）
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">成功后是否应用变更</param>
        /// <param name="cancellationToken">取消记号</param>
        /// <returns>受影响的行数</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            return this.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken, true);
        }

        /// <summary>
        /// 保存实体变更
        /// </summary>
        /// <returns>受影响的行数</returns>
        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken(), bool isSoftDelete = true)
        {
            return this.SaveChangesAsync(true, cancellationToken, isSoftDelete);
        }

        /// <summary>
        /// 保存实体变更
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">成功后是否应用变更</param>
        /// <param name="cancellationToken">取消记号</param>
        /// <param name="isSoftDelete">是否使用软删除</param>
        /// <returns>受影响的行数</returns>
        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken(), bool isSoftDelete = true)
        {
            this.SetSpecialPropertiesOfEntity();

            var changes = _entityHistoryRecorder?.DiscoveryChanges(this);
            
            if (isSoftDelete)
            {
                this.SetSoftDelete();
            }

            var i = base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            _entityHistoryRecorder?.RecordHistory(changes, isSoftDelete, i);
            return i;
        }

        public void StartRecord()
        {
            _entityHistoryRecorder?.StartRecord();
        }

        public void StopRecord()
        {
            _entityHistoryRecorder?.StopRecord();
        }
    }
}
