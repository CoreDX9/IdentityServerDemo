namespace CoreDX.Application.EntityFrameworkCore.Extensions
{
    //public static class DbContextExtensions
    //{
    //    private static readonly EntityState[] States = { EntityState.Added, EntityState.Deleted, EntityState.Modified };

    //    public static void SetSpecialPropertiesOfEntity(this DbContext dbContext)
    //    {
    //        var now = DateTimeOffset.Now;
    //        PropertyEntry propertyEntry;
    //        //在添加记录时设置创建时间并设置最后一次修改时间为创建时间、设置最后一次记录修改人为记录创建人
    //        foreach (var entry in dbContext.ChangeTracker.Entries().Where(e =>
    //            e.Metadata.ClrType.IsDerivedFrom(typeof(ILastModificationTimeRecordable)) && States.Contains(e.State)))
    //        {
    //            //在添加或修改实体时设置最后修改时间
    //            propertyEntry = entry.Property(nameof(ILastModificationTimeRecordable.LastModificationTime));
    //            propertyEntry.CurrentValue = now;
    //            propertyEntry.IsModified = true;

    //            if (entry.State == EntityState.Added)
    //            {
    //                propertyEntry = entry.Property(nameof(IEntity.CreationTime));
    //                propertyEntry.CurrentValue = now;
    //            }
    //            else
    //            {
    //                propertyEntry = entry.Property(nameof(IEntity.CreationTime));
    //                propertyEntry.IsModified = false;
    //            }

    //            //阻止在添加和修改实体时设置删除标记
    //            //但可以恢复被标记为已删除的记录
    //            if (entry.State != EntityState.Deleted
    //                && (bool) entry.Property(nameof(IEntity.IsDeleted)).CurrentValue == true)
    //            {
    //                propertyEntry = entry.Property(nameof(IEntity.IsDeleted));
    //                propertyEntry.CurrentValue = false;
    //                propertyEntry.IsModified = false;
    //            }

    //            //在启用、停用标记为null时认为不更改数据库值
    //            //添加时除外，添加时为null保持IsModified = true激活数据库默认值的插入
    //            if (entry.State != EntityState.Added
    //                && (bool?) entry.Property(nameof(IEntity.IsEnable)).CurrentValue == null)
    //            {
    //                propertyEntry = entry.Property(nameof(IEntity.IsEnable));
    //                propertyEntry.IsModified = false;
    //            }
    //        }
    //    }

    //    public static void SetSoftDelete(this DbContext dbContext)
    //    {
    //        PropertyEntry propertyEntry;
    //        foreach (var entry in dbContext.ChangeTracker.Entries().Where(e =>
    //            e.Metadata.ClrType.IsDerivedFrom(typeof(IEntity)) && e.State == EntityState.Deleted))
    //        {
    //            //在删除实体时修改删除标记并设置实体为修改用于软删除
    //            entry.State = EntityState.Modified;

    //            propertyEntry = entry.Property(nameof(IEntity.IsDeleted));
    //            propertyEntry.CurrentValue = true;
    //            propertyEntry.IsModified = true;
    //            propertyEntry.IsTemporary = false;
    //        }
    //    }

    //    public static void SetCreatorOrEditor(this DbContext dbContext, HttpContext httpContext)
    //    {
    //        PropertyEntry propertyEntry;
    //        foreach (var entry in dbContext.ChangeTracker.Entries().Where(e =>
    //            e.Metadata.ClrType.IsDerivedFrom(typeof(IDomainEntity<>)) && States.Contains(e.State)))
    //        {
    //            var subjectId = GetSubjectId(httpContext.User.Identity);
    //            var userId = ParseUserId(subjectId,
    //                entry.Property(nameof(IDomainEntity<int>.CreationUserId))?.Metadata.ClrType
    //            );
    //            //设置最后编辑用户Id
    //            propertyEntry = entry.Property(nameof(IDomainEntity<int>.LastModificationUserId));
    //            propertyEntry.CurrentValue = userId;
    //            propertyEntry.IsModified = true;
    //            propertyEntry.IsTemporary = false;

    //            //在增加时设置创建用户Id
    //            if (entry.State == EntityState.Added)
    //            {
    //                propertyEntry = entry.Property(nameof(IDomainEntity<int>.CreationUserId));
    //                propertyEntry.CurrentValue = userId;
    //                propertyEntry.IsModified = true;
    //                propertyEntry.IsTemporary = false;
    //            }
    //            else
    //            {
    //                propertyEntry = entry.Property(nameof(IDomainEntity<int>.CreationUserId));
    //                propertyEntry.IsModified = false;
    //            }
    //        }

    //        object ParseUserId(string id, Type type)
    //        {
    //            if (id.IsNullOrEmpty() || type == null)
    //            {
    //                return null;
    //            }

    //            if (type.IsDerivedFrom(typeof(Nullable<>)))
    //            {
    //                type = type.GenericTypeArguments[0];
    //            }

    //            object userId;
    //            if (type == typeof(Guid))
    //            {
    //                userId = new Guid(GetSubjectId(httpContext.User.Identity));
    //            }
    //            else if (type == typeof(int))
    //            {
    //                userId = int.Parse(id);
    //            }
    //            else if(type == typeof(long))
    //            {
    //                userId = long.Parse(id);
    //            }
    //            else if(type == typeof(string))
    //            {
    //                userId = id;
    //            }
    //            else
    //            {
    //                return null;
    //            }

    //            return userId;
    //        }
    //    }

    //    private static string GetSubjectId(IIdentity identity)
    //    {
    //        var id = identity as ClaimsIdentity;
    //        var claim = id?.FindFirst("sub");

    //        return claim?.Value;
    //    }
    //}
}
