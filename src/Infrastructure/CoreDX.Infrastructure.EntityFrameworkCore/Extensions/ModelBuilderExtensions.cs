using System;
using CoreDX.Application.Domain.Model.Entity;
using CoreDX.Application.Domain.Model.Entity.Core;
using CoreDX.Common.Util.TypeExtensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreDX.Infrastructure.EntityFrameworkCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static PropertyBuilder<DateTimeOffset> ConfigForICreationTime<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ICreationTimeRecordable
        {
            return builder.Property(e => e.CreationTime).ValueGeneratedOnAdd();
        }

        public static PropertyBuilder<DateTimeOffset> ConfigForILastModificationTime<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ILastModificationTimeRecordable
        {
            return builder.Property(e => e.LastModificationTime).ValueGeneratedOnAddOrUpdate();
        }

        public static EntityTypeBuilder<TEntity> ConfigQueryFilterForILogicallyDelete<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ILogicallyDeletable
        {
            return builder.HasQueryFilter(e => e.IsDeleted == false);
        }

        public static PropertyBuilder<string> ConfigForIOptimisticConcurrencySupported<TEntity>(
            this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IOptimisticConcurrencySupported
        {
            return builder.Property(e => e.ConcurrencyStamp).IsConcurrencyToken();
        }

        public static void ConfigForIDomainEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IDomainEntity
        {
            builder.ConfigForICreationTime();
            builder.ConfigForILastModificationTime();
            builder.ConfigQueryFilterForILogicallyDelete();
        }

        public static ReferenceCollectionBuilder<TIdentityUser, TEntity> ConfigCreator<TEntity, TIdentityUser, TIdentityKey>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class
            where TIdentityUser : class, IEntity<TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            if (typeof(TEntity).IsDerivedFrom(typeof(ICreatorRecordable<TIdentityKey, TIdentityUser>)))
            {
                return builder
                    .HasOne(e => ((ICreatorRecordable<TIdentityKey, TIdentityUser>)e).Creator)
                    .WithMany()
                    .HasForeignKey(e => ((ICreatorRecordable<TIdentityKey, TIdentityUser>)e).CreatorId);
            }

            throw new InvalidOperationException($"TEntity 不实现 ICreatorRecordable<{nameof(TIdentityKey)}, {nameof(TIdentityUser)}> 接口。");
        }

        public static ReferenceCollectionBuilder<TIdentityUser, TEntity> ConfigLastModifier<TEntity, TIdentityUser, TIdentityKey>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class
            where TIdentityUser : class, IEntity<TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            if (typeof(TEntity).IsDerivedFrom(typeof(ILastModifierRecordable<TIdentityKey, TIdentityUser>)))
            {
                return builder
                    .HasOne(e => ((ILastModifierRecordable<TIdentityKey, TIdentityUser>)e).LastModifier)
                    .WithMany()
                    .HasForeignKey(e => ((ILastModifierRecordable<TIdentityKey, TIdentityUser>)e).LastModifierId);
            }

            throw new InvalidOperationException("TEntity 不实现 ILastModifierRecordable<{nameof(TIdentityKey)}, {nameof(TIdentityUser)}> 接口。");
        }

        public static void ConfigForDomainEntityBase<TKey, TEntity, TIdentityUser, TIdentityKey>(
            this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainEntityBase<TKey, TIdentityKey>
            where TIdentityUser : class, IEntity<TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigForIOptimisticConcurrencySupported();
            builder.ConfigCreator<TEntity, TIdentityUser, TIdentityKey>();
            builder.ConfigLastModifier<TEntity, TIdentityUser, TIdentityKey>();
        }

        public static ReferenceCollectionBuilder<TEntity, TEntity> ConfigParentForIDomainTreeEntity<TKey, TEntity>(EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : class, IDomainTreeEntity<TKey, TEntity>
        {
            return builder.HasOne(e => e.Parent).WithMany(pe => pe.Children).HasForeignKey(e => e.ParentId);
        }
    }
}
