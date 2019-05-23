using System;
using CoreDX.Application.Domain.Model.Entity.Identity;

namespace CoreDX.Application.Domain.Model.Entity.Security
{
    /// <summary>
    /// 角色权限声明
    /// </summary>
    public class RolePermissionDeclaration : RolePermissionDeclaration<Guid> {}

    /// <summary>
    /// 用户权限声明
    /// </summary>
    public class UserPermissionDeclaration : UserPermissionDeclaration<Guid> {}

    /// <summary>
    /// 组织权限声明
    /// </summary>
    public class OrganizationPermissionDeclaration : OrganizationPermissionDeclaration<Guid> {}
}
