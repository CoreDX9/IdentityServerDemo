using System;

namespace CoreDX.Domain.Core.Entity
{
    /// <summary>
    /// 实体接口
    /// </summary>
    public interface IEntity {}

    /// <summary>
    /// 泛型实体接口，约束Id属性
    /// </summary>
    public interface IEntity<TKey> : IEntity
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
