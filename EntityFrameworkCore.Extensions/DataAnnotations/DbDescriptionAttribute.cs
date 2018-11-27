using System;

namespace EntityFrameworkCore.Extensions.DataAnnotations
{
    /// <summary>
    /// 实体在数据库中的表和列的说明
    /// 在迁移的Up方法中调用（确保在所有表创建和修改完成后，避免找不到表和列）
    /// migrationBuilder.ApplyDatabaseDescription(Migration m);
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DbDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 初始化新的实例
        /// </summary>
        /// <param name="description">说明内容</param>
        public DbDescriptionAttribute(string description) => Description = description;

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; }
    }
}
