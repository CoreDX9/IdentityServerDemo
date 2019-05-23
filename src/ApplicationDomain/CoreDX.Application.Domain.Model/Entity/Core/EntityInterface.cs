using System;
using System.ComponentModel.DataAnnotations;

namespace CoreDX.Application.Domain.Model.Entity.Core
{
    public interface ILogicallyDeletable
    {
        /// <summary>
        /// 逻辑删除标记
        /// </summary>
        bool IsDeleted { get; set; }
    }

    public interface IActiveControllable
    {
        /// <summary>
        /// 活动状态标记
        /// </summary>
        bool? Active { get; set; }
    }

    public interface IOptimisticConcurrencySupported
    {
        /// <summary>
        /// 行版本，乐观并发锁
        /// </summary>
        [ConcurrencyCheck]
        string ConcurrencyStamp { get; set; }
    }

    public interface IStorageOrderRecordable
    {
        /// <summary>
        /// 非自增顺序字段作为主键类型
        /// 应该在此列建立聚集索引避免随机的字段值导致数据库索引性能下降
        /// 同时保存数据插入先后的信息
        /// </summary>
        long InsertOrder { get; set; }
    }

    public interface ICreationTimeRecordable
    {
        /// <summary>
        /// 实体创建时间
        /// </summary>
        DateTimeOffset CreationTime { get; set; }
    }

    public interface ILastModificationTimeRecordable
    {
        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        DateTimeOffset LastModificationTime { get; set; }
    }

    public interface ICreatorRecordable<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct , IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        /// <summary>
        /// 创建人Id
        /// </summary>
        TIdentityKey? CreatorId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        TIdentityUser Creator { get; set; }
    }

    public interface ILastModifierRecordable<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
        /// <summary>
        /// 上一次修改人Id
        /// </summary>
        TIdentityKey? LastModifierId { get; set; }

        /// <summary>
        /// 上一次修改人
        /// </summary>
        TIdentityUser LastModifier { get; set; }
    }
}
