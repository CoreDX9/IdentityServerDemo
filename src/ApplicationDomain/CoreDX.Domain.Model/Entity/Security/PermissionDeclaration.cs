using System;
using CoreDX.Domain.Core.Entity;

namespace CoreDX.Domain.Model.Entity.Security
{
    /// <summary>
    /// 角色权限声明
    /// </summary>
    public class RolePermissionDeclaration : RolePermissionDeclaration<Guid, Guid>
        ,IStorageOrderRecordable
    {
        public virtual long InsertOrder { get; set; }
    }

    /// <summary>
    /// 用户权限声明
    /// </summary>
    public class UserPermissionDeclaration : UserPermissionDeclaration<Guid, Guid>
        , IStorageOrderRecordable
    {
        public virtual long InsertOrder { get; set; }
    }

    /// <summary>
    /// 组织权限声明
    /// </summary>
    public class OrganizationPermissionDeclaration : OrganizationPermissionDeclaration<Guid, Guid>
        , IStorageOrderRecordable
    {
        public virtual long InsertOrder { get; set; }
    }
}
