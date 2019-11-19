using CoreDX.Domain.Model.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoreDX.Domain.Entity.Permission
{
    /// <summary>
    /// 请求处理器识别特性，配合权限系统使用，整个系统中的名称不能重复,遗留代码引用太多，清理完后删除掉
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class RequestHandlerIdentificationAttribute : Attribute
    {
        /// <summary>
        /// 初始化新的实例
        /// </summary>
        /// <param name="uniqueKey">名称</param>
        public RequestHandlerIdentificationAttribute(string uniqueKey) => UniqueKey = uniqueKey;

        /// <summary>
        /// 唯一键
        /// </summary>
        public virtual string UniqueKey { get; }
    }

    public class RequestAuthorizationRule : RequestAuthorizationRule<int, int, AuthorizationRule, RequestAuthorizationRule>
    {
    }

    /// <summary>
    /// 请求授权规则
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TIdentityKey">身份主键类型</typeparam>
    public abstract class RequestAuthorizationRule<TKey, TIdentityKey, TAuthorizationRule, TRequestAuthorizationRule> : DomainEntityBase<TKey, TIdentityKey>
        where TKey : struct, IEquatable<TKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TRequestAuthorizationRule : RequestAuthorizationRule<TKey, TIdentityKey, TAuthorizationRule, TRequestAuthorizationRule>
        where TAuthorizationRule : AuthorizationRule<TKey, TIdentityKey, TRequestAuthorizationRule, TAuthorizationRule>
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
        public string IdentificationKey { get; set; }

        /// <summary>
        /// 授权规则Id
        /// </summary>
        public virtual TKey? AuthorizationRuleId { get; set; }

        /// <summary>
        /// 授权规则
        /// </summary>
        public virtual TAuthorizationRule AuthorizationRule { get; set; }
    }

    /// <summary>
    /// 授权规则
    /// </summary>
    public class AuthorizationRule : AuthorizationRule<int, int, RequestAuthorizationRule, AuthorizationRule>
    {
    }

    /// <summary>
    /// 授权规则
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TIdentityKey">身份主键类型</typeparam>
    public abstract class AuthorizationRule<TKey, TIdentityKey, TRequestAuthorizationRule, TAuthorizationRule> : DomainEntityBase<TKey, TIdentityKey>
        where TKey : struct, IEquatable<TKey>
        where TIdentityKey : struct, IEquatable<TIdentityKey>
        where TAuthorizationRule : AuthorizationRule<TKey, TIdentityKey, TRequestAuthorizationRule, TAuthorizationRule>
        where TRequestAuthorizationRule : RequestAuthorizationRule<TKey, TIdentityKey, TAuthorizationRule, TRequestAuthorizationRule>
    {
        public string Name { get; set; }

        /// <summary>
        /// 授权规则配置JSON，反序列化为AuthorizationRuleGroup类型
        /// </summary>
        public string AuthorizationRuleConfigJson { get; set; }

        private AuthorizationRuleGroup _authorizationRuleConfig;
        public AuthorizationRuleGroup AuthorizationRuleConfig => _authorizationRuleConfig ?? (_authorizationRuleConfig = JsonConvert.DeserializeObject<AuthorizationRuleGroup>(AuthorizationRuleConfigJson));

        public virtual List<TRequestAuthorizationRule> RequestAuthorizationRules { get; set; } = new List<TRequestAuthorizationRule>();

        /// <summary>
        /// 授权规则组
        /// </summary>
        public class AuthorizationRuleGroup
        {
            /// <summary>
            /// 操作
            /// </summary>
            public enum Operate : byte
            {
                /// <summary>
                /// 任意
                /// </summary>
                Any = 1,

                /// <summary>
                /// 所有
                /// </summary>
                All = 2,
            }

            /// <summary>
            /// 组操作，表示组中任意一个规则或组还是所有规则和组通过视为本组授权通过
            /// </summary>
            public Operate GroupOperate { get; set; }

            /// <summary>
            /// 规则集合
            /// </summary>
            public List<Rule> Rules { get; set; }

            /// <summary>
            /// 内部分组集合
            /// </summary>
            public List<AuthorizationRuleGroup> Groups { get; set; }
        }

        /// <summary>
        /// 授权规则
        /// </summary>
        public class Rule
        {
            /// <summary>
            /// 权限来源
            /// </summary>
            public class PermissionOrigin
            {
                /// <summary>
                /// 来源类型
                /// </summary>
                public enum PermissionOriginType : byte
                {
                    User = 1,
                    Role = 2,
                    Organization = 3,
                    Anonymous = 200,
                    Authentication = 201
                }

                /// <summary>
                /// 来源类型
                /// </summary>
                public PermissionOriginType Type { get; set; }

                /// <summary>
                /// 值，用于指定权限来自指定的角色或组织，针对用户、匿名、身份认证的无效，集合为空表不限制
                /// 组织权限是收缩模型，下级组织不自动继承上级组织的权限，需要手动配置继承，且继承的权限不能超过上级组织
                /// 设置后用户必须直接属于指定组织且指定组织直接拥有指定权限，为下级声明上级组织中没有的新权限是高度敏感操作
                /// 角色权限是扩张模型，子角色会自动继承父角色的权限，除非重新声明相同权限覆盖继承的权限
                /// 这些都是在程序中执行的逻辑，存储模型没有区别，要达到相应效果需要用代码控制
                /// </summary>
                public List<Guid> Values { get; set; }
            }

            /// <summary>
            /// 权限定义Id
            /// </summary>
            public int PermissionDefinitionId { get; set; }

            /// <summary>
            /// 权限值
            /// </summary>
            public short Value { get; set; }

            /// <summary>
            /// 权限来源，只要其中一个来源能够匹配即可
            /// </summary>
            public List<PermissionOrigin> Origins { get; set; }
        }
    }
}
