using System;
using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Model.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreDX.EntityFrameworkCore.Extensions.Domain
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// 配置实体创建时间仅在插入记录时写入数据库
        /// 避免意外在应用中被修改
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>属性构造器</returns>
        public static PropertyBuilder<DateTimeOffset> ConfigForICreationTime<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ICreationTimeRecordable
        {
            return builder.Property(e => e.CreationTime);//.ValueGeneratedOnAdd();
        }

        /// <summary>
        /// 配置实体最后修改时间在插入或修改记录时写入数据库
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>属性构造器</returns>
        public static PropertyBuilder<DateTimeOffset> ConfigForILastModificationTime<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ILastModificationTimeRecordable
        {
            return builder.Property(e => e.LastModificationTime);//.ValueGeneratedOnAddOrUpdate();
        }

        /// <summary>
        /// 配置可软删除实体的查询过滤器让ef core自动添加查询条件过滤已被软删除的记录
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigQueryFilterForILogicallyDelete<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ILogicallyDeletable
        {
            return builder.HasQueryFilter(e => e.IsDeleted == false);
        }

        /// <summary>
        /// 配置乐观并发实体的并发检查字段
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>属性构造器</returns>
        public static PropertyBuilder<string> ConfigForIOptimisticConcurrencySupported<TEntity>(
            this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IOptimisticConcurrencySupported
        {
            return builder.Property(e => e.ConcurrencyStamp).IsConcurrencyToken();
        }

        /// <summary>
        /// 配置领域实体接口
        /// 包括创建时间、上次修改时间、软删除过滤器
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForIDomainEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IDomainEntity
        {
            builder.ConfigForICreationTime();
            builder.ConfigForILastModificationTime();
            builder.ConfigQueryFilterForILogicallyDelete();

            return builder;
        }

        /// <summary>
        /// 配置创建人导航属性和外键
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TIdentityUser">创建人类型</typeparam>
        /// <typeparam name="TIdentityKey">创建人主键类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>导航集合构造器</returns>
        public static ReferenceCollectionBuilder<TIdentityUser, TEntity> ConfigForICreatorRecordable<TEntity, TIdentityUser, TIdentityKey>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ICreatorRecordable<TIdentityKey, TIdentityUser>
            where TIdentityUser : class, IEntity<TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            return builder
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatorId);
        }

        /// <summary>
        /// 配置上次修改人导航属性和外键
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TIdentityUser">上次修改人类型</typeparam>
        /// <typeparam name="TIdentityKey">上次修改人主键类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>导航集合构造器</returns>
        public static ReferenceCollectionBuilder<TIdentityUser, TEntity> ConfigForILastModifierRecordable<TEntity, TIdentityUser, TIdentityKey>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ILastModifierRecordable<TIdentityKey, TIdentityUser>
            where TIdentityUser : class, IEntity<TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            return builder
                .HasOne(e => e.LastModifier)
                .WithMany()
                .HasForeignKey(e => e.LastModifierId);
        }

        /// <summary>
        /// 配置多对多导航实体接口
        /// 包括创建时间
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForIManyToManyReferenceEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IManyToManyReferenceEntity
        {
            builder.ConfigForICreationTime();

            return builder;
        }

        /// <summary>
        /// 配置多对多导航实体
        /// 包括创建时间
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForManyToManyReferenceEntityBase<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : ManyToManyReferenceEntityBase
        {
            return builder.ConfigForIManyToManyReferenceEntity();
        }

        /// <summary>
        /// 配置多对多导航实体接口
        /// 包括创建时间
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TIdentityKey">身份主键类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForIManyToManyReferenceEntity<TEntity, TIdentityKey>(this EntityTypeBuilder<TEntity> builder)
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TEntity : class, IManyToManyReferenceEntity<TIdentityKey>
        {
            return builder.ConfigForIManyToManyReferenceEntity();
        }

        /// <summary>
        /// 配置多对多导航实体接口
        /// 包括创建时间
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TIdentityKey">身份主键类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForManyToManyReferenceEntityBase<TEntity, TIdentityKey>(this EntityTypeBuilder<TEntity> builder)
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TEntity : ManyToManyReferenceEntityBase<TIdentityKey>
        {
            return builder.ConfigForIManyToManyReferenceEntity<TEntity, TIdentityKey>();
        }

        /// <summary>
        /// 配置多对多导航实体
        /// 包括创建时间、创建人导航和外键
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TIdentityKey">身份主键类型</typeparam>
        /// <typeparam name="TIdentityUser">身份类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForIManyToManyReferenceEntity<TEntity, TIdentityKey, TIdentityUser>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IManyToManyReferenceEntity<TIdentityKey, TIdentityUser>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TIdentityUser : class, IEntity<TIdentityKey>
        {
            builder.ConfigForIManyToManyReferenceEntity<TEntity, TIdentityKey>()
                .ConfigForICreatorRecordable<TEntity, TIdentityUser, TIdentityKey>();

            return builder;
        }

        /// <summary>
        /// 配置多对多导航实体
        /// 包括创建时间、创建人导航和外键
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TIdentityKey">身份主键类型</typeparam>
        /// <typeparam name="TIdentityUser">身份类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForManyToManyReferenceEntityBase<TEntity, TIdentityKey, TIdentityUser>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : ManyToManyReferenceEntityBase<TIdentityKey, TIdentityUser>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TIdentityUser : class, IEntity<TIdentityKey>
        {
            return builder.ConfigForIManyToManyReferenceEntity<TEntity, TIdentityKey, TIdentityUser>();
        }

        /// <summary>
        /// 配置领域实体
        /// 包括创建时间、上次修改时间、软删除过滤器、乐观并发检查
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForDomainEntityBase<TKey, TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainEntityBase<TKey>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigForIOptimisticConcurrencySupported();

            return builder;
        }

        /// <summary>
        /// 配置领域实体
        /// 包括创建时间、上次修改时间、软删除过滤器、乐观并发检查、
        /// 创建人导航属性和外键、上次修改人导航属性和外键
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TIdentityKey">身份主键类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForDomainEntityBase<TKey, TEntity, TIdentityKey>(
            this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainEntityBase<TKey, TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            return builder.ConfigForDomainEntityBase<TKey, TEntity>();
        }

        /// <summary>
        /// 配置领域实体
        /// 包括创建时间、上次修改时间、软删除过滤器、乐观并发检查、
        /// 创建人导航属性和外键、上次修改人导航属性和外键
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TIdentityUser">身份实体类型</typeparam>
        /// <typeparam name="TIdentityKey">身份实体主键类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForDomainEntityBase<TKey, TEntity, TIdentityUser, TIdentityKey>(
            this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainEntityBase<TKey, TIdentityKey, TIdentityUser>
            where TIdentityUser : class, IEntity<TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TEntity>();
            builder.ConfigForICreatorRecordable<TEntity, TIdentityUser, TIdentityKey>();
            builder.ConfigForILastModifierRecordable<TEntity, TIdentityUser, TIdentityKey>();

            return builder;
        }

        /// <summary>
        /// 配置树形领域实体接口
        /// 包括自关联导航属性和外键
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TEntity">树形实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>导航集合构造器</returns>
        public static ReferenceCollectionBuilder<TEntity, TEntity> ConfigParentForIDomainTreeEntity<TKey, TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : class, IDomainTreeEntity<TKey, TEntity>
        {
            builder.HasAnnotation("IsTreeEntity", "");
            return builder.HasOne(e => e.Parent)
                .WithMany(pe => pe.Children)
                .HasForeignKey(e => e.ParentId);
        }

        /// <summary>
        /// 配置树形领域实体接口
        /// 包括创建时间、上次修改时间、软删除过滤器、自关联导航属性和外键
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TEntity">树形实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForIDomainTreeEntity<TKey, TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : class, IDomainTreeEntity<TKey, TEntity>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigParentForIDomainTreeEntity<TKey, TEntity>();

            return builder;
        }

        /// <summary>
        /// 配置树形领域实体
        /// 包括创建时间、上次修改时间、软删除过滤器、乐观并发检查、自关联导航属性和外键
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TEntity">树形实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForDomainTreeEntityBase<TKey, TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainTreeEntityBase<TKey, TEntity>
        {
            builder.ConfigForDomainEntityBase<TKey, TEntity>();
            builder.ConfigParentForIDomainTreeEntity<TKey, TEntity>();

            return builder;
        }

        /// <summary>
        /// 配置树形领域实体
        /// 包括创建时间、上次修改时间、软删除过滤器、乐观并发检查、自关联导航属性和外键
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TEntity">树形实体类型</typeparam>
        ///   <typeparam name="TIdentityKey">身份实体主键类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForDomainTreeEntityBase<TKey, TEntity, TIdentityKey>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainTreeEntityBase<TKey, TEntity>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            builder.ConfigForDomainTreeEntityBase<TKey, TEntity>();

            return builder;
        }

        /// <summary>
        /// 配置树形领域实体
        /// 包括创建时间、上次修改时间、软删除过滤器、乐观并发检查、自关联导航属性和外键、
        /// 创建人导航属性和外键、上次修改人导航属性和外键
        /// </summary>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <typeparam name="TEntity">树形实体类型</typeparam>
        /// <typeparam name="TIdentityKey">身份实体主键类型</typeparam>
        /// <typeparam name="TIdentityUser">身份实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        /// <returns>实体类型构造器</returns>
        public static EntityTypeBuilder<TEntity> ConfigForDomainTreeEntityBase<TKey, TEntity, TIdentityKey, TIdentityUser>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainTreeEntityBase<TKey, TEntity, TIdentityKey, TIdentityUser>
            where TIdentityUser : class, IEntity<TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            builder.ConfigForDomainTreeEntityBase<TKey, TEntity, TIdentityKey>();
            builder.ConfigForICreatorRecordable<TEntity, TIdentityUser, TIdentityKey>();
            builder.ConfigForILastModifierRecordable<TEntity, TIdentityUser, TIdentityKey>();

            return builder;
        }
    }
}
