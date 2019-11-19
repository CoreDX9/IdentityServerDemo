using CoreDX.Common.Util.TypeExtensions;
using CoreDX.Domain.Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreDX.Domain.Model.Repository
{
    public static class EntityEntryExtensions
    {
        public static void ProcessCreationInfoWhenModified(this IEntity entity, DbContext dbContext)
        {
            var entityEntry = dbContext.ChangeTracker.Entries().Single(x => x == entity);
            if (entity is ICreationTimeRecordable)
            {
                var creationTime = entityEntry
                    .Property(nameof(ICreationTimeRecordable.CreationTime));
                creationTime.IsTemporary = true;
                creationTime.IsModified = false;
            }

            if (entity.GetType().IsDerivedFrom(typeof(ICreatorRecordable<>)))
            {
                var creator = entityEntry
                    .Property(nameof(ICreatorRecordable<bool>.CreatorId));
                creator.IsTemporary = true;
                creator.IsModified = false;
            }
        }

        public static void GenerateNewConcurrencyStamp(this IOptimisticConcurrencySupported entity)
        {
            entity.ConcurrencyStamp = Guid.NewGuid().ToString();
        }
    }
}
