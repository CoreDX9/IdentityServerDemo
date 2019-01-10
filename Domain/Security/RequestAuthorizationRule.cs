using System;
using System.Collections.Generic;
using Domain.Identity;

namespace Domain.Security
{
    /// <summary>
    /// 请求处理器识别特性，配合权限系统使用，整个系统中的名称不能重复
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class RequestHandlerIdentificationAttribute : Attribute
    {
        /// <summary>
        /// 初始化新的实例
        /// </summary>
        /// <param name="name">名称</param>
        public RequestHandlerIdentificationAttribute(string name) => Name = name;

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; }
    }

    /// <summary>
    /// 请求授权规则
    /// </summary>
    public class RequestAuthorizationRule : DomainEntityBase<Guid, Guid>
    {
        /// <summary>
        /// 处理方法签名
        /// </summary>
        public string HandlerMethodSignature { get; set; }

        /// <summary>
        /// 处理方法所在类型全名
        /// </summary>
        public string TypeFullName { get; set; }

        /// <summary>
        /// 请求处理方法识别名称
        /// 自动从RequestHandlerIdentificationAttribute提取
        /// 如果存在，但签名和所在类型全名不匹配则更新签名和类型全名为RequestHandlerIdentificationAttribute所标记的方法
        /// 如果不存在，则以签名和类型名为准，如果没有匹配的请求处理器，则视为无效授权规则
        /// 所以如果希望在修改方法名、参数、类名、命名空间等时保持授权规则不会失效，请为请求处理器标注RequestHandlerIdentificationAttribute特性
        /// </summary>
        public string IdentificationName { get; set; }

        /// <summary>
        /// 授权规则配置JSON
        /// </summary>
        public string AuthorizationRuleConfigJson { get; set; }

        //public virtual List<RequestHandlerPermissionDeclaration> PermissionDeclarations { get; set; } = new List<RequestHandlerPermissionDeclaration>();
    }

    //public class RequestHandlerPermissionDeclarationRole : DomainEntityBase<Guid, Guid>
    //{
    //    public Guid? RoleId { get; set; }

    //    public virtual ApplicationRole Role { get; set; }

    //    public Guid? PermissionDeclarationId { get; set; }

    //    public virtual RequestHandlerPermissionDeclaration PermissionDeclaration { get; set; }
    //}

    //public class RequestHandlerPermissionDeclarationOrganization : DomainEntityBase<Guid, Guid>
    //{
    //    public Guid? OrganizationId { get; set; }

    //    public virtual Organization Organization { get; set; }

    //    public Guid? PermissionDeclarationId { get; set; }

    //    public virtual RequestHandlerPermissionDeclaration PermissionDeclaration { get; set; }
    //}
}
