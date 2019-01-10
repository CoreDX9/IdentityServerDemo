using System;
using System.Linq;
using System.Linq.Expressions;
using Domain.Identity;
using Domain.Sample;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Domain.EntityFrameworkCore.EntityTypeConfigurations;
using Util.TypeExtensions;

namespace Domain.EntityFrameworkCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// 配置实现了IDomainEntity&lt;&gt;接口的实体
        /// </summary>
        /// <param name="modelBuilder">模型构造器</param>
        /// <param name="dbContext">ef上下文</param>
        /// <returns>传入的模型构造器</returns>
        public static ModelBuilder ConfigEntitiesThatImplementedFromIDomainEntity(this ModelBuilder modelBuilder, DbContext dbContext)
        {
            //遍历所有实现了IDomainEntity<>接口的DbSet<>的实体
            //如果实体是通过配置类或反射方式进行配置的，此方式扫描不到
            //此方式只能扫描到硬编码的类型
            //var domains = dbContext.GetType().GetProperties()
            //    .Where(pro => pro.PropertyType.CanBeReferencedBy(typeof(DbSet<>)))
            //    .Select(pro => pro.PropertyType.GenericTypeArguments[0])
            //    .Where(t => t.CanBeReferencedBy(typeof(IDomainEntity<>)));

            //循环配置模型通用部分，如有其它特殊配置，在下面循环之后自行增加配置
            //里面的int类型参数只是因为nameof表达式中的泛型类型必须是封闭类型，故而随便填一个int，不会影响属性名
            //IDomainEntity<>接口实现了IEntity接口
            foreach (var domain in modelBuilder.Model.GetEntityTypes().Where(e =>
                    !e.IsQueryType && !e.IsOwned() && e.ClrType.IsDerivedFrom(typeof(IDomainEntity<>)))
                .Select(e => e.ClrType))
            {
                modelBuilder.Entity(domain, b =>
                {
                    //如果实体继承自DomainEntityBase<,>并且主键Id为Guid类型，则添加字符串转换器（用来兼容非sqlserver数据库，efcore2.1以上支持）
                    if (domain.IsDerivedFrom(typeof(DomainEntityBase<,>)) &&
                        domain.GetProperty(nameof(DomainEntityBase<int, int>.Id))?.PropertyType == typeof(Guid))
                    {
                        b.Property(nameof(DomainEntityBase<int, int>.Id)).HasConversion(new GuidToStringConverter());
                    }

                    //配置记录创建时间默认值，仅限sqlserver
                    b.Property(nameof(IEntity.CreationTime)).ValueGeneratedOnAdd();

                    if (dbContext.Database.IsSqlServer())
                    {
                        b.Property(nameof(IEntity.CreationTime)).HasDefaultValueSql("sysDateTimeOffset()");
                    }

                    //如果ef已经自动设置了标识列，什么都不做，否则把OrderNumber设置为自增长标识列
                    if (!b.Metadata.GetProperties()
                        .Any(p => p.Name != nameof(IEntity.OrderNumber) &&
                                  p.SqlServer().ValueGenerationStrategy ==
                                  SqlServerValueGenerationStrategy.IdentityColumn))
                    {
                        var orderNumberMetadata = b.Property(nameof(IEntity.OrderNumber)).Metadata;
                        orderNumberMetadata.SqlServer().ValueGenerationStrategy =
                            SqlServerValueGenerationStrategy.IdentityColumn;
                        orderNumberMetadata.ValueGenerated = ValueGenerated.OnAddOrUpdate;
                        //由数据库生成值并忽略变更，不然会导致ef报列插入更新异常
                        orderNumberMetadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                        //设置索引
                        //b.HasIndex(nameof(IEntity.OrderNumber)).IsUnique();
                    }

                    //配置记录默认为启用状态
                    b.Property(nameof(IEntity.IsEnable)).IsRequired().HasDefaultValue(true);

                    if (dbContext.Database.IsSqlServer())
                    {
                        b.Property(nameof(IEntity.IsEnable)).HasDefaultValueSql("'True'");
                    }

                    if (dbContext.Database.IsSqlServer())
                    {
                        b.Property(nameof(IEntity.IsDeleted)).HasDefaultValueSql("'False'");
                    }

                    //配置乐观并发列
                    b.Property(nameof(IEntity.RowVersion)).IsRowVersion();

                    //配置记录创建人外键
                    b.HasOne(typeof(ApplicationUser), nameof(IDomainEntity<int>.CreationUser))
                        .WithMany()
                        .HasForeignKey(nameof(IDomainEntity<int>.CreationUserId));

                    //配置记录上次修改人外键
                    b.HasOne(typeof(ApplicationUser), nameof(IDomainEntity<int>.LastModificationUser))
                        .WithMany()
                        .HasForeignKey(nameof(IDomainEntity<int>.LastModificationUserId));

                    //配置查询过滤器过滤掉被标记为已删除的记录
                    //例如 b.HasQueryFilter((Expression<Func<IdentityUser, bool>>) (u => !u.IsDeleted));
                    b.HasQueryFilter(FilterSoftDeletedEntryFor(domain));
                });
            }

            return modelBuilder;
        }

        /// <summary>
        /// 配置继承自DomainTreeEntityViewBase&lt;TKey, TEntityView , TIdentityUserKey&gt;的实体视图
        /// </summary>
        /// <param name="modelBuilder">模型构造器</param>
        /// <returns>传入的模型构造器</returns>
        public static ModelBuilder ConfigTreeViewsThatSubClassOfDomainTreeEntityViewBase(this ModelBuilder modelBuilder)
        {
            var treeViews = modelBuilder.Model.GetEntityTypes()
                .Where(en => en.IsQueryType && en.ClrType.IsDerivedFrom(typeof(DomainTreeEntityViewBase<,,>)))
                .Select(en => en.ClrType);
            //var treeViews = dbContext.GetType().GetProperties()
            //    .Where(pro => pro.PropertyType.CanBeReferencedBy(typeof(DbQuery<>)))
            //    .Select(pro => pro.PropertyType.GenericTypeArguments[0])
            //    .Where(t => t.CanBeReferencedBy(typeof(DomainTreeEntityViewBase<,,>)));

            foreach (var view in treeViews)
            {
                modelBuilder.Query(view, b =>
                {
                    b.HasQueryFilter(FilterSoftDeletedEntryFor(view));
                    //b.ToView($"view_{b.Metadata.Relational().TableName}");
                });
            }

            return modelBuilder;
        }

        /// <summary>
        /// 创建查询过滤器过滤被标记为已删除的记录
        /// </summary>
        /// <param name="domainType">实体类型</param>
        /// <returns>查询表达式</returns>
        private static LambdaExpression FilterSoftDeletedEntryFor(Type domainType)
        {
            ParameterExpression parameter = Expression.Parameter(domainType, "domain");//lambda表示式里的参数表达式

            Expression left;//相当于 a=b 的 a
            Expression right;//相当于a=b 的 b
            Expression e1;//作为最后形成的表达式的载体,相当于 a=b

            //domain.IsDeleted
            left = Expression.Property(parameter, domainType.GetProperty(nameof(IEntity.IsDeleted)));
            //Constant方法设置属性对应的值，字面量表达式
            right = Expression.Constant(false);
            //domain.IsDeleted == false
            e1 = Expression.Equal(left, right);

            // ((DomainEntityBase<,>)domain) => (domain.IsDeleted == false)
            // 这里的IEntity会在运行时反射为实体的实际类型，这些实体都实现了IEntity接口
            var filter = Expression.Lambda(e1, parameter);

            //domain.IsEnable
            //left = Expression.Property(parameter, domainType.GetProperty(nameof(IEntity.IsEnable)));
            //right = Expression.Constant(true);
            //domain.IsEnable == true
            //Expression e2 = Expression.Equal(left, right);

            // (domain.IsDeleted == false && domain.IsEnable == true)
            //Expression predicateBody = Expression.AndAlso(e1, e2);

            // ((IEntity)domain) => (domain.IsDeleted == false && domain.IsEnable == true)
            //var filter = Expression.Lambda(predicateBody, parameter);

            return filter;
        }

        /// <summary>
        /// 配置Identity
        /// </summary>
        /// <param name="modelBuilder">模型构造器</param>
        /// <returns>传入的模型构造器</returns>
        public static ModelBuilder ConfigIdentity(this ModelBuilder modelBuilder)
        {
            #region Identity映射配置

            if (typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Id))?.PropertyType == typeof(Guid))
            {
                modelBuilder.Entity<ApplicationUser>(b =>
                {
                    b.Property(u => u.Id).HasConversion(new GuidToStringConverter());
                });
            }

            if (typeof(ApplicationRole).GetProperty(nameof(ApplicationRole.Id))?.PropertyType == typeof(Guid))
            {
                modelBuilder.Entity<ApplicationRole>(b =>
                {
                    b.Property(u => u.Id).HasConversion(new GuidToStringConverter());
                });
            }

            #endregion

            #region Identity导航属性配置

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            #endregion

            #region Identity表名配置

            modelBuilder.Entity<ApplicationUser>(b => b.ToTable("AppUsers"));
            modelBuilder.Entity<ApplicationUserClaim>(b => b.ToTable("AppUserClaims"));
            modelBuilder.Entity<ApplicationUserLogin>(b => b.ToTable("UserLogins"));
            modelBuilder.Entity<ApplicationUserToken>(b => b.ToTable("AppUserTokens"));
            modelBuilder.Entity<ApplicationRole>(b => b.ToTable("AppRoles"));
            modelBuilder.Entity<ApplicationRoleClaim>(b => b.ToTable("AppRoleClaims"));
            modelBuilder.Entity<ApplicationUserRole>(b => b.ToTable("AppUserRoles"));

            #endregion

            return modelBuilder;
        }

        /// <summary>
        /// 配置实现了ITree&lt;&gt;接口的实体视图
        /// 需要在迁移中创建视图
        /// 调用Domain.EntityFrameworkCore.MigrationBuilderExtensions.CreateTreeEntityView(this MigrationBuilder mb, Migration m, Assembly a)
        /// </summary>
        /// <param name="modelBuilder">模型构造器</param>
        /// <returns>传入的模型构造器</returns>
        public static ModelBuilder ConfigViewsThatImplementedFromITree(this ModelBuilder modelBuilder)
        {
            modelBuilder.Query<ApplicationRoleView>(b =>
            {
                b.Ignore(rv => rv.Parent);
                b.ToView("view_AppRoles");
            });
            modelBuilder.Query<TreeDomainView>(b =>
            {
                b.Ignore(rv => rv.Parent);
                b.ToView("view_TreeDomains");
            });

            return modelBuilder;
        }

        public static ModelBuilder ConfigPermission(this ModelBuilder modelBuilder)
        {
            return modelBuilder.ApplyConfiguration(new PermissionDefinitionConfig())
                .ApplyConfiguration(new RolePermissionDeclarationConfig())
                .ApplyConfiguration(new UserPermissionDeclarationConfig())
                .ApplyConfiguration(new OrganizationPermissionDeclarationConfig())
                //.ApplyConfiguration(new RequestHandlerPermissionDeclarationConfig())
                .ApplyConfiguration(new RequestAuthorizationRuleConfig());
                //.ApplyConfiguration(new RequestHandlerPermissionDeclarationRoleConfig())
                //.ApplyConfiguration(new RequestHandlerPermissionDeclarationOrganizationConfig());
        }
    }
}
