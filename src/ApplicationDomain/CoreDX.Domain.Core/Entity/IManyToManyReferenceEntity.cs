using System;

namespace CoreDX.Domain.Core.Entity
{
    /// <summary>
    /// 多对多导航实体接口
    /// </summary>
    /// <typeparam name="TIdentityKey">身份实体主键类型</typeparam>
    /// <typeparam name="TIdentityUser">身份实体类型</typeparam>
    public interface IManyToManyReferenceEntity<TIdentityKey, TIdentityUser> : IManyToManyReferenceEntity<TIdentityKey>
        , ICreatorRecordable<TIdentityKey, TIdentityUser>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TIdentityUser : IEntity<TIdentityKey>
    {
    }

    /// <summary>
    /// 多对多导航实体接口
    /// </summary>
    /// <typeparam name="TIdentityKey">身份实体主键类型</typeparam>
    public interface IManyToManyReferenceEntity<TIdentityKey> : IManyToManyReferenceEntity
        , ICreatorRecordable<TIdentityKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
    {
    }

    /// <summary>
    /// 多对多导航实体接口
    /// </summary>
    public interface IManyToManyReferenceEntity : IEntity
        , ICreationTimeRecordable
    {
    }
}
