using CoreDX.Application.Repository.EntityFrameworkCore;
using CoreDX.Common.Util.TypeExtensions;
using CoreDX.Domain.Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Repository.EntityFrameworkCore
{
    public class EFCoreRepository<TEntity, TKey, TDbContext> : EFCoreRepository<TEntity, TDbContext>, IEFCoreRepository<TEntity, TKey, TDbContext>
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

        public override TEntity Find(TEntity entity)
        {
            return Find(entity.Id);
        }

        public virtual Task<TEntity> FindAsync(TKey key)
        {
            return dbSet.FindAsync(key).AsTask();
        }

        public override Task<TEntity> FindAsync(TEntity entity)
        {
            return FindAsync(entity.Id);
        }
    }

    public class EFCoreRepository<TEntity, TDbContext> : IEFCoreRepository<TEntity, TDbContext>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext dbContext;
        protected readonly DbSet<TEntity> dbSet;

        protected virtual void ProcessChangedEntity()
        {
            var changedEntities = dbContext.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Added);
            foreach (var entity in changedEntities)
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
                            if (entity is IActiveControllable)
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
                    default:
                        break;
                }
            }
        }

        protected virtual void ResetDeletedMark(params TEntity[] entities)
        {
            foreach (var entity in entities)
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
            if (isSoftDelete)
            {
                if (entity is ILogicallyDeletable)
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
            foreach (var entity in entities)
            {
                Delete(entity, isSoftDelete);
            }
        }

        public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool isSoftDelete, CancellationToken cancellationToken = default)
        {
            DeleteRange(entities, isSoftDelete);
            return Task.CompletedTask;
        }

        public virtual TEntity Find(TEntity entity)
        {
            var exp = GenerateWhere(dbContext, entity);

            return Set.SingleOrDefault(exp);
        }

        public virtual Task<TEntity> FindAsync(TEntity entity)
        {
            var exp = GenerateWhere(dbContext, entity);

            return Set.SingleOrDefaultAsync(exp);
        }

        public virtual int SaveChanges()
        {
            ProcessChangedEntity();
            return dbContext.SaveChanges();
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProcessChangedEntity();
            return dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual IQueryable<TEntity> Set => dbSet.AsNoTracking();

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

        static private Expression<Func<TEntity, bool>> GenerateWhere(TDbContext dbContext, TEntity entity)
        {
            //查找实体类型主键
            var model = dbContext.Model.FindEntityType(typeof(TEntity));
            var key = model.FindPrimaryKey();

            //查找所有主键属性，如果没有主键就使用所有实体属性
            IEnumerable<PropertyInfo> props;
            if (key != null)
            {
                props = key.Properties.Select(x => x.PropertyInfo);
            }
            else
            {
                props = model.GetProperties().Select(x => x.PropertyInfo);
            }

            //生成表达式参数
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");

            //初始化提取实体类型所有属性信息生成属性访问表达式并包装备用
            var keyValues = props.Select(x => new { key = x, value = x.GetValue(entity), propExp = Expression.Property(parameter, x) });

            //初始化存储由基础类型组成的属性信息（只要个空集合，实际数据在后面的循环中填充）
            var primitiveKeyValues = keyValues.Take(0).Where(x => IsPrimitiveType(x.key.PropertyType));
            //初始化基础类型属性的相等比较表达式存储集合（只要个空集合，实际数据在后面的循环中填充）
            var equals = primitiveKeyValues.Take(0).Select(x => Expression.Equal(x.propExp, Expression.Constant(x.value)));
            //初始化复杂类型属性存储集合
            var notPrimitiveKeyValues = primitiveKeyValues;

            //如果还有元素，说明上次用于提取信息的复杂属性内部还存在复杂属性，接下来用提取到的基础类型属性信息生成相等比较表达式并合并到存储集合然后继续提取剩下的复杂类型属性的内部属性
            while (keyValues.Count() > 0)
            {
                //提取由基础类型组成的属性信息
                primitiveKeyValues = keyValues.Where(x => IsPrimitiveType(x.key.PropertyType));
                //生成基础类型属性的相等比较表达式
                equals = equals.Concat(primitiveKeyValues.Select(x => Expression.Equal(x.propExp, Expression.Constant(x.value))));
                //提取复杂类型属性
                notPrimitiveKeyValues = keyValues.Except(primitiveKeyValues);
                //分别提取各个复杂类型属性内部的属性信息继续生成内部属性访问表达式
                keyValues =
                    from kv in notPrimitiveKeyValues
                    from propInfo in kv.value.GetType().GetProperties()
                    select new { key = propInfo, value = propInfo.GetValue(kv.value), propExp = Expression.Property(kv.propExp, propInfo) };
            }

            //如果相等比较表达式有多个，将所有相等比较表达式用 && 运算连接起来
            var and = equals.First();
            foreach (var eq in equals.Skip(1))
            {
                and = Expression.AndAlso(and, eq);
            }

            //生成完整的过滤条件表达式，形如：  (TEntity x) => { return x.a == ? && x.b == ? && x.obj1.m == ? && x.obj1.n == ? && x.obj2.u.v == ?; }
            var exp = Expression.Lambda<Func<TEntity, bool>>(and, parameter);

            //判断某个类型是否是基础数据类型
            static bool IsPrimitiveType(Type type)
            {
                var primitiveTypes = new[] {
                    typeof(sbyte)
                    ,typeof(byte)
                    ,typeof(short)
                    ,typeof(ushort)
                    ,typeof(int)
                    ,typeof(uint)
                    ,typeof(long)
                    ,typeof(ulong)
                    ,typeof(float)
                    ,typeof(double)
                    ,typeof(decimal)
                    ,typeof(char)
                    ,typeof(string)
                    ,typeof(bool)
                    ,typeof(DateTime)
                    ,typeof(DateTimeOffset)
                    //,typeof(Enum)
                    ,typeof(Guid)};

                var tmp =
                    type.IsDerivedFrom(typeof(Nullable<>))
                    ? Nullable.GetUnderlyingType(type)
                    : type;

                return tmp.IsEnum || primitiveTypes.Contains(tmp);
            }

            return exp;
        }
    }
}
