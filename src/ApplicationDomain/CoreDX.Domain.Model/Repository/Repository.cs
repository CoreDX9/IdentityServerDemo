using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Core.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Model.Repository
{
    public abstract class EFCoreRepository<TEntity, TKey, TDbContext> : EFCoreRepository<TEntity, TDbContext>, IBulkOperateVariableRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
        where TDbContext : DbContext
    {
        public EFCoreRepository(TDbContext dbContext) : base(dbContext)
        {
        }

        public virtual void Delete(TKey key, bool isSoftDelete)
        {
            var entity = Find(key);
            Delete(entity, isSoftDelete);
        }

        public virtual Task DeleteAsync(TKey key, bool isSoftDelete, CancellationToken cancellationToken = default)
        {
            Delete(key, isSoftDelete);
            return Task.CompletedTask;
        }

        public virtual void DeleteRange(IEnumerable<TKey> keys, bool isSoftDelete)
        {
            var entities = Find(keys).ToArray();
            dbSet.AttachRange(entities);
            DeleteRange(entities, isSoftDelete);
        }

        public virtual Task DeleteRangeAsync(IEnumerable<TKey> keys, bool isSoftDelete, CancellationToken cancellationToken = default)
        {
            DeleteRange(keys, isSoftDelete);
            return Task.CompletedTask;
        }

        public virtual TEntity Find(TKey key)
        {
            return dbSet.AsNoTracking().SingleOrDefault(x => x.Id.Equals(key));
        }

        public virtual IQueryable<TEntity> Find(IEnumerable<TKey> keys)
        {
            return dbSet.AsNoTracking().Where(x => keys.Contains(x.Id));
        }

        public virtual Task<TEntity> FindAsync(TKey key)
        {
            return dbSet.FindAsync(key).AsTask();
        }
    }

    public abstract class EFCoreRepository<TEntity, TDbContext> : IBulkOperateVariableRepository<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext dbContext;
        protected readonly DbSet<TEntity> dbSet;

        protected virtual void ProcessChangedEntity()
        {
            var changedEntities = dbContext.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Added);
            foreach(var entity in changedEntities)
            {
                (entity as IOptimisticConcurrencySupported)?.GenerateNewConcurrencyStamp();
            }

            var changedEntitiesGroups = changedEntities.GroupBy(x => x.State);
            foreach (var group in changedEntitiesGroups)
            {
                switch (group)
                {
                    case var entities when entities.Key == EntityState.Added:
                        foreach (var entity in entities)
                        {
                            if(entity is IActiveControllable)
                            {
                                var active = (entity as IActiveControllable).Active;
                                (entity as IActiveControllable).Active = active ?? true;
                            }
                        }
                        break;
                    case var entities when entities.Key == EntityState.Modified:
                        foreach (var entity in entities)
                        {
                            (entity as IEntity)?.ProcessCreationInfoWhenModified(dbContext);

                            if (entity is IActiveControllable && (entity as IActiveControllable).Active == null)
                            {
                                entity.Property(nameof(IActiveControllable.Active)).IsModified = false;
                            }
                        }
                        break;
                    default :
                        break;
                }
            }
        }

        protected virtual void ResetDeletedMark(params TEntity[] entities)
        {
            foreach(var entity in entities)
            {
                if (entity is ILogicallyDeletable)
                {
                    (entity as ILogicallyDeletable).IsDeleted = false;
                }
            }
        }

        public EFCoreRepository(TDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = this.dbContext.Set<TEntity>();
        }

        public virtual void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return dbSet.AddAsync(entity, cancellationToken).AsTask();
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Delete(TEntity entity, bool isSoftDelete)
        {
            dbSet.Attach(entity);
            if(isSoftDelete)
            {
                if(entity is ILogicallyDeletable)
                {
                    (entity as ILogicallyDeletable).IsDeleted = true;
                }
                else
                {
                    throw new InvalidOperationException($"要求软删除的实体不实现{nameof(ILogicallyDeletable)}接口。");
                }
            }
            else
            {
                dbSet.Remove(entity);
            }
        }

        public virtual Task DeleteAsync(TEntity entity, bool isSoftDelete, CancellationToken cancellationToken = default)
        {
            Delete(entity, isSoftDelete);
            return Task.CompletedTask;
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool isSoftDelete)
        {
            dbSet.AttachRange(entities);
            foreach(var entity in entities)
            {
                Delete(entity, isSoftDelete);
            }
        }

        public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool isSoftDelete, CancellationToken cancellationToken = default)
        {
            DeleteRange(entities, isSoftDelete);
            return Task.CompletedTask;
        }

        public abstract TEntity Find(TEntity entity);

        public abstract Task<TEntity> FindAsync(TEntity entity);

        public virtual void SaveChanges()
        {
            ProcessChangedEntity();
            dbContext.SaveChanges();
        }

        public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProcessChangedEntity();
            return dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual IQueryable<TEntity> Set()
        {
            return dbSet.AsNoTracking();
        }

        public virtual void Update(TEntity entity)
        {
            ResetDeletedMark(entity);
            dbSet.Update(entity);
        }

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Update(entity);
            return Task.CompletedTask;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            ResetDeletedMark(entities.ToArray());
            dbSet.UpdateRange(entities);
        }

        public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            UpdateRange(entities);
            return Task.CompletedTask;
        }
    }
}
