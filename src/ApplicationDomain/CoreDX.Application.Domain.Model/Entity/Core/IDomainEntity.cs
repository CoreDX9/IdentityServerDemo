using System;
using System.ComponentModel;

namespace CoreDX.Application.Domain.Model.Entity.Core
{
    public interface IDomainEntity
        : IEntity
            , ILogicallyDeletable
            , IActiveControllable
            , IOptimisticConcurrencySupported
            , ICreationTimeRecordable
            , ILastModificationTimeRecordable
            , INotifyPropertyChanged
            , IPropertyChangeTrackable
    {
    }

    public interface IDomainEntity<TKey>
        : IEntity<TKey>, IDomainEntity
        where TKey : struct, IEquatable<TKey>
    { }
}
