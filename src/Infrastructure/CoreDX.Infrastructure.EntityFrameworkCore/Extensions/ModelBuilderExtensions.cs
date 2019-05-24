using System;
using CoreDX.Application.Domain.Model.Entity;
using CoreDX.Application.Domain.Model.Entity.Core;
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

        public static void ConfigForIManyToManyReferenceEntity<TEntity, TIdentityKey, TIdentityUser>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IManyToManyReferenceEntity<TIdentityKey, TIdentityUser>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
            where TIdentityUser : class, IEntity<TIdentityKey>
        {
            builder.ConfigForICreationTime();
            builder.ConfigForICreatorRecordable<TEntity, TIdentityUser, TIdentityKey>();
        }

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

        public static void ConfigForDomainEntityBase<TKey, TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainEntityBase<TKey>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigForIOptimisticConcurrencySupported();
        }

        public static void ConfigForDomainEntityBase<TKey, TEntity, TIdentityUser, TIdentityKey>(
            this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : DomainEntityBase<TKey, TIdentityKey, TIdentityUser>
            where TIdentityUser : class, IEntity<TIdentityKey>
            where TIdentityKey : struct, IEquatable<TIdentityKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TEntity>();
            builder.ConfigForICreatorRecordable<TEntity, TIdentityUser, TIdentityKey>();
            builder.ConfigForILastModifierRecordable<TEntity, TIdentityUser, TIdentityKey>();
        }

        public static ReferenceCollectionBuilder<TEntity, TEntity> ConfigParentForIDomainTreeEntity<TKey, TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct, IEquatable<TKey>
            where TEntity : class, IDomainTreeEntity<TKey, TEntity>
        {
            return builder.HasOne(e => e.Parent).WithMany(pe => pe.Children).HasForeignKey(e => e.ParentId);
        }

        public static void ConfigForIDomainTreeEntity<TKey, TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TKey : struct , IEquatable<TKey>
            where TEntity : class, IDomainTreeEntity<TKey, TEntity>
        {
            builder.ConfigForIDomainEntity();
            builder.ConfigParentForIDomainTreeEntity<TKey, TEntity>();
        }
    }
}
