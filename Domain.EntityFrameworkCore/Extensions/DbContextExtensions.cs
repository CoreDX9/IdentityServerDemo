using System;
using System.Linq;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace Domain.EntityFrameworkCore.Extensions
{
    public static class DbContextExtensions
    {
        public static void SetSpecialPropertiesOfEntity(this DbContext dbContext)
        {
            var states = new[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };
            var now = DateTimeOffset.Now;
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e=>states.Contains(e.State)))
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IEntity.CreationTime) &&
                        p.Metadata.ClrType == typeof(DateTimeOffset)))
                    {
                        entry.Property(nameof(IEntity.CreationTime)).CurrentValue = now;
                    }

                    if (entry.Properties.Any(p =>
                            p.Metadata.Name == nameof(IDomainEntity<int>.CreationUserId))
                        && entry.Properties.Any(p =>
                            p.Metadata.Name == nameof(IDomainEntity<int>.LastModificationUserId))
                        && entry.Property(nameof(IDomainEntity<int>.LastModificationUserId)).Metadata.ClrType ==
                        entry.Property(nameof(IDomainEntity<int>.CreationUserId)).Metadata.ClrType)
                    {
                        //entry.Property(nameof(IDomainEntity<int>.LastModificationUser)).CurrentValue =
                        //    entry.Property(nameof(IDomainEntity<int>.CreationUser)).CurrentValue;

                        entry.Property(nameof(IDomainEntity<int>.LastModificationUserId)).CurrentValue =
                            entry.Property(nameof(IDomainEntity<int>.CreationUserId)).CurrentValue;
                    }
                }
                else
                {
                    if (entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IEntity.CreationTime)))
                    {
                        entry.Property(nameof(IEntity.CreationTime)).IsModified = false;
                    }

                    if (entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IDomainEntity<int>.CreationUserId)))
                    {
                        //entry.Property(nameof(IDomainEntity<int>.CreationUser)).IsModified = false;
                        entry.Property(nameof(IDomainEntity<int>.CreationUserId)).IsModified = false;
                    }
                }

                //在添加或修改实体时设置最后修改时间
                if (entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IEntity.LastModificationTime) && p.Metadata.ClrType == typeof(DateTimeOffset)))
                {
                    entry.Property(nameof(IEntity.LastModificationTime)).CurrentValue = now;
                    entry.Property(nameof(IEntity.LastModificationTime)).IsModified = true;
                }

                //阻止在添加和修改实体时设置删除标记
                //但可以恢复被标记为已删除的记录
                if (entry.State != EntityState.Deleted
                    && entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IEntity.IsDeleted) && p.Metadata.ClrType == typeof(bool))
                    && (bool)entry.Property(nameof(IEntity.IsDeleted)).CurrentValue == true)
                {
                    entry.Property(nameof(IEntity.IsDeleted)).CurrentValue = false;
                    entry.Property(nameof(IEntity.IsDeleted)).IsModified = false;
                }

                //在启用、停用标记为null时认为不更改数据库值
                //添加时除外，添加时为null保持IsModified = true激活数据库默认值的插入
                if (entry.State != EntityState.Added && entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IEntity.IsEnable) && p.Metadata.ClrType == typeof(bool?))
                    && (bool?)entry.Property(nameof(IEntity.IsEnable)).CurrentValue == null)
                {
                    entry.Property(nameof(IEntity.IsEnable)).IsModified = false;
                }
            }
        }

        public static void SetSoftDelete(this DbContext dbContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e=>e.State == EntityState.Deleted))
            {
                //在删除实体时修改删除标记并设置实体为修改用于软删除
                if (entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IEntity.IsDeleted) && p.Metadata.ClrType == typeof(bool)))
                {
                    entry.Property(nameof(IEntity.IsDeleted)).CurrentValue = true;
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(IEntity.IsDeleted)).IsModified = true;
                    entry.Property(nameof(IEntity.IsDeleted)).IsTemporary = false;
                }
            }
        }
    }
}
