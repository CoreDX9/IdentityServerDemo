using CoreDX.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Repository
{
    public interface IBulkOperateRepository
    {
        void SaveChanges();
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }

    public interface IRepository<TEntity> : IVariableRepository<TEntity>, IReadOnlyRepository<TEntity>
        where TEntity : IEntity
    {
    }

    public interface IRepository<TEntity, TKey> : IVariableRepository<TEntity, TKey>, IReadOnlyRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    public interface IVariableRepository<TEntity>
        where TEntity : IEntity
    {
        void Add(TEntity entity);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        void Delete(TEntity entity, bool isSoftDelete);
        Task DeleteAsync(TEntity entity, bool isSoftDelete, CancellationToken cancellationToken);

        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        void UpdateRange(IEnumerable<TEntity> entities);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        void DeleteRange(IEnumerable<TEntity> entities, bool isSoftDelete);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool isSoftDelete, CancellationToken cancellationToken);
    }
    public interface IVariableRepository<TEntity, TKey> : IVariableRepository<TEntity>
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        void Delete(TKey key, bool isSoftDelete);
        Task DeleteAsync(TKey key, bool isSoftDelete, CancellationToken cancellationToken);
        void DeleteRange(IEnumerable<TKey> keys, bool isSoftDelete);
        Task DeleteRangeAsync(IEnumerable<TKey> keys, bool isSoftDelete, CancellationToken cancellationToken);
    }
    public interface IBulkOperateVariableRepository<TEntity> : IVariableRepository<TEntity>, IBulkOperateRepository
         where TEntity : IEntity
    {
    }

    public interface IBulkOperateVariableRepository<TEntity, TKey> : IBulkOperateVariableRepository<TEntity>
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
    }

    public interface IReadOnlyRepository<TEntity>
        where TEntity : IEntity
    {
        IQueryable<TEntity> Set();
        TEntity Find(TEntity entity);
        Task<TEntity> FindAsync(TEntity entity);

    }
    public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity>
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        TEntity Find(TKey key);
        Task<TEntity> FindAsync(TKey key);
        IQueryable<TEntity> Find(IEnumerable<TKey> keys);
    }
}
