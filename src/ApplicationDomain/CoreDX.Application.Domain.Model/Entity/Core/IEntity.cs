using System;
using System.ComponentModel.DataAnnotations;

namespace CoreDX.Application.Domain.Model.Entity.Core
{
    public interface IEntity
    { }

    public interface IEntity<TKey> : IEntity
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
