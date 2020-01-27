using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Core.Repository;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoreDX.Domain.Repository.EntityFrameworkCore
{
    public interface IEFCoreRepository<TEntity, TDbContext> : IReadOnlyRepository<TEntity>, IBulkOperateVariableRepository<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    { }
    public interface IEFCoreRepository<TEntity, TKey, TDbContext> : IEFCoreRepository<TEntity, TDbContext>, IReadOnlyRepository<TEntity, TKey>, IBulkOperateVariableRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
        where TDbContext : DbContext
    { }
}
