namespace CoreDX.Domain.Entity.Permission
{
    /// <summary>
    /// 角色权限声明
    /// </summary>
    public class RolePermissionDeclaration : RolePermissionDeclaration<int, int , PermissionDefinition>
    {
    }

    /// <summary>
    /// 用户权限声明
    /// </summary>
    public class UserPermissionDeclaration : UserPermissionDeclaration<int, int, PermissionDefinition>
    {
    }

    /// <summary>
    /// 组织权限声明
    /// </summary>
    public class OrganizationPermissionDeclaration : OrganizationPermissionDeclaration<int, int, PermissionDefinition>
    {
    }
}
