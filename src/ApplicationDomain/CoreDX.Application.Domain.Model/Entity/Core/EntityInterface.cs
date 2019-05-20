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
        [Timestamp]
        byte[] RowVersion { get; set; }
    }

    public interface IStorageOrderRecordable
    {
        /// <summary>
        /// 如果选用GUID作为主键类型
        /// 应该在此列建立聚合索引避免随机的GUID导致数据库索引性能下降
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

    public interface ICreatorRecordable<TIdentityUserKey, TIdentityUser>
    where TIdentityUserKey : struct , IEquatable<TIdentityUserKey>
    where TIdentityUser : IEntity<TIdentityUserKey>
    {
        TIdentityUserKey? CreatorId { get; set; }

        TIdentityUser Creator { get; set; }
    }

    public interface ILastModifierRecordable<TIdentityUserKey, TIdentityUser>
        where TIdentityUserKey : struct, IEquatable<TIdentityUserKey>
        where TIdentityUser : IEntity<TIdentityUserKey>
    {
        TIdentityUserKey? LastModifierId { get; set; }

        TIdentityUser LastModifier { get; set; }
    }
}
