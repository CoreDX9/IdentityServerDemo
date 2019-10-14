using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using CoreDX.Common.Util.TypeExtensions;
using CoreDX.Domain.Core.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        private static readonly EntityState[] States = { EntityState.Added, EntityState.Deleted, EntityState.Modified };

        public static void SetSpecialPropertiesOfEntity(this DbContext dbContext)
        {
            var now = DateTimeOffset.Now;
            PropertyEntry propertyEntry;

            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e => States.Contains(e.State)))
            {
                if (entry.Metadata.ClrType.IsDerivedFrom(typeof(ILastModificationTimeRecordable)))
                {
                    //在添加或修改实体时设置最后修改时间
                    propertyEntry = entry.Property(nameof(ILastModificationTimeRecordable.LastModificationTime));
                    propertyEntry.CurrentValue = now;
                    propertyEntry.IsModified = true;
                }

                if (entry.Metadata.ClrType.IsDerivedFrom(typeof(ICreationTimeRecordable)))
                {
                    if (entry.State == EntityState.Added)
                    {
                        propertyEntry = entry.Property(nameof(ICreationTimeRecordable.CreationTime));
                        propertyEntry.CurrentValue = now;
                    }
                    else
                    {
                        propertyEntry = entry.Property(nameof(ICreationTimeRecordable.CreationTime));
                        propertyEntry.IsModified = false;
                    }
                }

                if (entry.Metadata.ClrType.IsDerivedFrom(typeof(ILogicallyDeletable)))
                {
                    //阻止在添加和修改实体时设置删除标记
                    //但可以恢复被标记为已删除的记录
                    if (entry.State != EntityState.Deleted
                        && (bool)entry.Property(nameof(ILogicallyDeletable.IsDeleted)).CurrentValue == true)
                    {
                        propertyEntry = entry.Property(nameof(ILogicallyDeletable.IsDeleted));
                        propertyEntry.CurrentValue = false;
                        propertyEntry.IsModified = false;
                    }
                }

                if (entry.Metadata.ClrType.IsDerivedFrom(typeof(IActiveControllable)))
                {
                    //在启用、停用标记为null时认为不更改数据库值
                    //添加时除外，添加时为null保持IsModified = true激活数据库默认值的插入
                    if (entry.State != EntityState.Added
                        && (bool?)entry.Property(nameof(IActiveControllable.Active)).CurrentValue == null)
                    {
                        propertyEntry = entry.Property(nameof(IActiveControllable.Active));
                        propertyEntry.IsModified = false;
                    }
                }
            }
        }

        public static void SetSoftDelete(this DbContext dbContext)
        {
            PropertyEntry propertyEntry;

            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e =>
                e.Metadata.ClrType.IsDerivedFrom(typeof(ILogicallyDeletable)) && e.State == EntityState.Deleted))
            {
                //在删除实体时修改删除标记并设置实体为修改用于软删除
                entry.State = EntityState.Modified;

                propertyEntry = entry.Property(nameof(ILogicallyDeletable.IsDeleted));
                propertyEntry.CurrentValue = true;
                propertyEntry.IsModified = true;
                propertyEntry.IsTemporary = false;
            }
        }

        public static void SetCreatorOrEditor(this DbContext dbContext, HttpContext httpContext)
        {
            PropertyEntry propertyEntry;
            foreach (var entry in dbContext.ChangeTracker.Entries().Where(e =>
                e.Metadata.ClrType.IsDerivedFrom(typeof(IDomainEntity<>)) && States.Contains(e.State)))
            {
                var subjectId = GetSubjectId(httpContext.User.Identity);
                var userId = ParseUserId(subjectId,
                    entry.Property(nameof(ICreatorRecordable<int>.CreatorId))?.Metadata.ClrType
                );
                //设置最后编辑用户Id
                propertyEntry = entry.Property(nameof(ILastModifierRecordable<int>.LastModifierId));
                propertyEntry.CurrentValue = userId;
                propertyEntry.IsModified = true;
                propertyEntry.IsTemporary = false;

                //在增加时设置创建用户Id
                if (entry.State == EntityState.Added)
                {
                    propertyEntry = entry.Property(nameof(ICreatorRecordable<int>.CreatorId));
                    propertyEntry.CurrentValue = userId;
                    propertyEntry.IsModified = true;
                    propertyEntry.IsTemporary = false;
                }
                else
                {
                    propertyEntry = entry.Property(nameof(ICreatorRecordable<int>.CreatorId));
                    propertyEntry.IsModified = false;
                }
            }

            object ParseUserId(string id, Type type)
            {
                if (id.IsNullOrEmpty() || type == null)
                {
                    return null;
                }

                if (type.IsDerivedFrom(typeof(Nullable<>)))
                {
                    type = type.GenericTypeArguments[0];
                }

                object userId;
                if (type == typeof(Guid))
                {
                    userId = new Guid(GetSubjectId(httpContext.User.Identity));
                }
                else if (type == typeof(int))
                {
                    userId = int.Parse(id);
                }
                else if(type == typeof(long))
                {
                    userId = long.Parse(id);
                }
                else if(type == typeof(string))
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

            return claim?.Value;
        }
    }
}
