using System;
using Domain.Identity;
using Entity;

namespace Domain
{
    /// <summary>
    /// 领域实体基类
    /// 提供实体创建人与最近实体修改者的存储支持
    /// </summary>
    /// <typeparam name="TIdentityUserKey">实体操作人的实体类型主键</typeparam>
    public interface IDomainEntity<TIdentityUserKey> : IEntity
        where TIdentityUserKey : struct, IEquatable<TIdentityUserKey>
    {
        /// <summary>
        /// 创建人Id
        /// </summary>
        TIdentityUserKey? CreationUserId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        //[ForeignKey(nameof(CreationUserId))]
        ApplicationUser CreationUser { get; set; }

        /// <summary>
        /// 最近修改人Id
        /// </summary>
        TIdentityUserKey? LastModificationUserId { get; set; }

        /// <summary>
        /// 最近修改人
        /// </summary>
        //[ForeignKey(nameof(LastModificationUserId))]
        ApplicationUser LastModificationUser { get; set; }
    }
}
