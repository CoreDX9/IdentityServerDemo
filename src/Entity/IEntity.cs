using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public interface IEntity
    {
        /// <summary>
        /// 如果选用GUID作为主键类型
        /// 应该在此列建立聚合索引避免随机的GUID导致数据库索引性能下降
        /// 同时保存数据插入先后的信息
        /// </summary>
        long OrderNumber { get; set; }

        /// <summary>
        /// 行版本，乐观并发锁
        /// </summary>
        [Timestamp]
        byte[] RowVersion { get; set; }

        /// <summary>
        /// 可用、停用标记
        /// </summary>
        bool? IsEnable { get; set; }

        /// <summary>
        /// 逻辑删除标记
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// 最后一次修改时间
        /// </summary>
        DateTimeOffset LastModificationTime { get; set; }
    }
}
