using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Util.TypeExtensions;

namespace Domain.EntityFrameworkCore.Extensions
{
    public static class DbContextExtensions
    {
        private static readonly EntityState[] States = { EntityState.Added, EntityState.Deleted, EntityState.Modified };

        public static void SetSpecialPropertiesOfEntity(this DbContext dbContext)
        {
            var now = DateTimeOffset.Now;
            //在添加记录时设置创建时间并设置最后一次修改时间为创建时间、设置最后一次记录修改人为记录创建人
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e=> States.Contains(e.State)))
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IEntity.CreationTime) &&
                        p.Metadata.ClrType == typeof(DateTimeOffset)))
                    {
                        entry.Property(nameof(IEntity.CreationTime)).CurrentValue = now;
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

        public static void SetCreatorOrEditor(this DbContext dbContext, HttpContext httpContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e => States.Contains(e.State)))
            {
                var subjectId = GetSubjectId(httpContext.User.Identity);
                var userId = ParseUserId(subjectId, entry);
                //设置最后编辑用户Id
                if (entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IDomainEntity<int>.LastModificationUserId))
                    && userId != null)
                {
                    entry.Property(nameof(IDomainEntity<int>.LastModificationUserId)).CurrentValue = userId;
                    entry.Property(nameof(IDomainEntity<int>.LastModificationUserId)).IsModified = true;
                    entry.Property(nameof(IDomainEntity<int>.LastModificationUserId)).IsTemporary = false;
                }

                //在增加时设置创建用户Id
                if (entry.Properties.Any(p =>
                        p.Metadata.Name == nameof(IDomainEntity<int>.CreationUserId)))
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(nameof(IDomainEntity<int>.CreationUserId)).CurrentValue = userId;
                        entry.Property(nameof(IDomainEntity<int>.CreationUserId)).IsModified = true;
                        entry.Property(nameof(IDomainEntity<int>.CreationUserId)).IsTemporary = false;
                    }
                    else
                    {
                        entry.Property(nameof(IDomainEntity<int>.CreationUserId)).IsModified = false;
                    }
                }
            }

            object ParseUserId(string id, EntityEntry entry)
            {
                if (id.IsNullOrEmpty())
                {
                    return null;
                }

                object userId;
                if (entry.Property(nameof(IDomainEntity<int>.CreationUserId)).Metadata.ClrType == typeof(Guid))
                {
                    userId = new Guid(GetSubjectId(httpContext.User.Identity));
                }
                else if (entry.Property(nameof(IDomainEntity<int>.CreationUserId)).Metadata.ClrType == typeof(int))
                {
                    userId = int.Parse(id);
                }
                else if(entry.Property(nameof(IDomainEntity<int>.CreationUserId)).Metadata.ClrType == typeof(long))
                {
                    userId = long.Parse(id);
                }
                else if(entry.Property(nameof(IDomainEntity<int>.CreationUserId)).Metadata.ClrType == typeof(string))
                {
                    userId = id;
                }
                else
                {
                    return null;
                }

                return userId;
            }
        }

        private static string GetSubjectId(IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id?.FindFirst("sub");

            if (claim == null) throw new InvalidOperationException("sub claim is missing");
            return claim.Value;
        }
    }
}
