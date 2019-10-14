using System;
using System.ComponentModel.DataAnnotations;

namespace CoreDX.Domain.Core.Entity
{
    /// <summary>
    /// 软删除接口
    /// </summary>
    public interface ILogicallyDeletable
    {
        /// <summary>
        /// 逻辑删除标记
        /// </summary>
        bool IsDeleted { get; set; }
    }

    /// <summary>
    /// 活动状态标记接口
    /// </summary>
    public interface IActiveControllable
    {
        /// <summary>
        /// 活动状态标记
        /// </summary>
        bool? Active { get; set; }
    }

    /// <summary>
    /// 乐观并发接口
    /// </summary>
    public interface IOptimisticConcurrencySupported
    {
        /// <summary>
        /// 行版本，乐观并发锁
        /// </summary>
        [ConcurrencyCheck]
        string ConcurrencyStamp { get; set; }
    }

    /// <summary>
    /// 插入顺序记录接口
    /// </summary>
    public interface IStorageOrderRecordable
    {
        /// <summary>
        /// 非自增顺序字段作为主键类型
        /// 应该在此列建立聚集索引避免随机的字段值导致数据库索引性能下降
        /// 同时保存数据插入先后的信息
        /// </summary>
        long InsertOrder { get; set; }
    }

    /// <summary>
    /// 创建时间记录接口
    /// </summary>
    public interface ICreationTimeRecordable
    {
        /// <summary>
        /// 实体创建时间
        /// </summary>
        DateTimeOffset CreationTime { get; set; }
    }

    /// <summary>
    /// 最后修改时间记录接口
    /// </summary>
    public interface ILastModificationTimeRecordable
    {
        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        DateTimeOffset LastModificationTime { get; set; }
    }

    /// <summary>
    /// 创建人id记录接口
    /// </summary>
    /// <typeparam name="TIdentityKey">创建人主键类型</typeparam>
    public interface ICreatorRecordable<TIdentityKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
        /// <summary>
        /// 创建人Id
        /// </summary>
        TIdentityKey? CreatorId { get; set; }
    }

    /// <summary>
    /// 创建人记录接口
    /// </summary>
    /// <typeparam name="TIdentityKey">创建人主键类型</typeparam>
    /// <typeparam name="TIdentityUser">创建人类型</typeparam>
    public interface ICreatorRecordable<TIdentityKey, TIdentityUser> : ICreatorRecordable<TIdentityKey>
        where TIdentityKey : struct , IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        /// <summary>
        /// 创建人
        /// </summary>
        TIdentityUser Creator { get; set; }
    }

    /// <summary>
    /// 上次修改人id记录接口
    /// </summary>
    /// <typeparam name="TIdentityKey">上次修改人主键类型</typeparam>
    public interface ILastModifierRecordable<TIdentityKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
        /// <summary>
        /// 上一次修改人Id
        /// </summary>
        TIdentityKey? LastModifierId { get; set; }
    }

    /// <summary>
    /// 上次修改人记录接口
    /// </summary>
    /// <typeparam name="TIdentityKey">上次修改人主键类型</typeparam>
    /// <typeparam name="TIdentityUser">上次修改人类型</typeparam>
    public interface ILastModifierRecordable<TIdentityKey, TIdentityUser> : ILastModifierRecordable<TIdentityKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        /// <summary>
        /// 上一次修改人
        /// </summary>
        TIdentityUser LastModifier { get; set; }
    }
}
