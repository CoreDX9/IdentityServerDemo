using System;
using System.ComponentModel;

namespace CoreDX.Domain.Core.Entity
{
    /// <summary>
    /// 领域实体接口，主要是整合各个小接口
    /// </summary>
    public interface IDomainEntity : IEntity
        , ILogicallyDeletable
        //, IActiveControllable
        , ICreationTimeRecordable
        , ILastModificationTimeRecordable
        , INotifyPropertyChanged
        , IPropertyChangeTrackable
    {}

    /// <summary>
    /// 泛型领域实体接口
    /// </summary>
    public interface IDomainEntity<TKey> : IEntity<TKey>
        , IDomainEntity
        where TKey : struct, IEquatable<TKey>
    {}
}
