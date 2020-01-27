using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreDX.Common.Util.TypeExtensions;
using CoreDX.EntityFrameworkCore.DataAnnotations;
using CoreDX.EntityFrameworkCore.Extensions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDX.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// 数据迁移扩展
    /// </summary>
    public static class MigrationBuilderExtensions
    {
        /// <summary>
        /// 添加或更新表说明
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">表名</param>
        /// <param name="description">说明</param>
        /// <param name="schema">架构</param>
        public static void AddOrUpdateTableDescription(this MigrationBuilder migrationBuilder, string tableName,
            string description, string schema = "dbo")
        {
            if (!description.IsNullOrWhiteSpace() && description.Contains('\'')) description = description.Replace("'", "''");
            migrationBuilder.Sql(MigrationSqlTemplate.AddTableDbDescriptionTemplate
                .Replace("{tableDescription}", description)
                .Replace("{schema}", schema)
                .Replace("{tableName}", tableName));
        }

        /// <summary>
        /// 删除表说明
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">表名</param>
        /// <param name="schema">架构</param>
        public static void DropTableDescription(this MigrationBuilder migrationBuilder, string tableName,
            string schema = "dbo")
        {
            migrationBuilder.Sql(MigrationSqlTemplate.DropTableDbDescriptionTemplate
                .Replace("{schema}", schema)
                .Replace("{tableName}", tableName));
        }

        /// <summary>
        /// 添加或更新列说明
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <param name="description">说明</param>
        /// <param name="schema">架构</param>
        public static void AddOrUpdateColumnDescription(this MigrationBuilder migrationBuilder, string tableName,
            string columnName, string description, string schema = "dbo")
        {
            if (!description.IsNullOrWhiteSpace() && description.Contains('\'')) description = description.Replace("'", "''");
            migrationBuilder.Sql(MigrationSqlTemplate.AddColumnDbDescriptionTemplate
                .Replace("{columnDescription}", description)
                .Replace("{schema}", schema)
                .Replace("{tableName}", tableName)
                .Replace("{columnName}", columnName));
        }

        /// <summary>
        /// 删除列说明
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <param name="schema">架构</param>
        public static void DropColumnDescription(this MigrationBuilder migrationBuilder, string tableName,
            string columnName, string schema = "dbo")
        {
            migrationBuilder.Sql(MigrationSqlTemplate.DropColumnDbDescriptionTemplate
                .Replace("{schema}", schema)
                .Replace("{tableName}", tableName)
                .Replace("{columnName}", columnName));
        }

        /// <summary>
        /// 自动扫描迁移模型并添加表和列说明（不推荐使用）
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="migration">迁移类实例</param>
        /// <param name="entityAssembly">实体类所在程序集</param>
        public static void ApplyDatabaseDescription(this MigrationBuilder migrationBuilder, Migration migration,
            Assembly entityAssembly)
        {
            var defaultSchema = "dbo";

            if (entityAssembly == null)
                throw new NullReferenceException($"{nameof(entityAssembly)} cannot be null.");

            foreach (var entityType in migration.TargetModel.GetEntityTypes().Select(entity =>
                new {EntityType = entity, ClrType = entityAssembly.GetType(entity.Name)}))
            {
                //添加表说明
                if (entityType.ClrType?.CustomAttributes.Any(
                        attr => attr.AttributeType == typeof(DbDescriptionAttribute)) == true)
                {
                    migrationBuilder.AddOrUpdateTableDescription(entityType.EntityType.GetTableName(),
                        (entityType.ClrType.GetCustomAttribute(typeof(DbDescriptionAttribute)) as DbDescriptionAttribute
                        )?.Description, entityType.EntityType.GetSchema().IsNullOrEmpty() ? defaultSchema : entityType.EntityType.GetSchema());
                }

                //添加列说明
                foreach (var property in entityType.EntityType.GetProperties())
                {
                    if (entityType.ClrType?.GetProperty(property.Name)?.CustomAttributes
                            .Any(attr => attr.AttributeType == typeof(DbDescriptionAttribute)) == true)
                    {
                        //如果该列的实体属性是枚举类型，把枚举的说明追加到列说明
                        var enumDbDescription = string.Empty;
                        if (entityType.ClrType?.GetProperty(property.Name)?.PropertyType.IsEnum == true
                            || (entityType.ClrType?.GetProperty(property.Name)?.PropertyType
                                    .IsDerivedFrom(typeof(Nullable<>)) == true
                                && entityType.ClrType?.GetProperty(property.Name)?.PropertyType.GenericTypeArguments[0]
                                    .IsEnum == true))
                        {
                            var @enum = entityType.ClrType?.GetProperty(property.Name)?.PropertyType
                                            .IsDerivedFrom(typeof(Nullable<>)) == true
                                ? entityType.ClrType?.GetProperty(property.Name)?.PropertyType.GenericTypeArguments[0]
                                : entityType.ClrType?.GetProperty(property.Name)?.PropertyType;

                            var descList = new List<string>();
                            foreach (var field in @enum?.GetFields() ?? new FieldInfo[0])
                            {
                                if (!field.IsSpecialName)
                                {
                                    var desc = (field.GetCustomAttributes(typeof(DbDescriptionAttribute), false)
                                        .FirstOrDefault() as DbDescriptionAttribute)?.Description;
                                    descList.Add(
                                        $@"{field.GetRawConstantValue()} : {(desc.IsNullOrWhiteSpace() ? field.Name : desc)}");
                                }
                            }

                            var isFlags = @enum?.GetCustomAttribute(typeof(FlagsAttribute)) != null;
                            var enumTypeDbDescription =
                                (@enum?.GetCustomAttributes(typeof(DbDescriptionAttribute), false).FirstOrDefault() as
                                    DbDescriptionAttribute)?.Description;
                            enumTypeDbDescription += enumDbDescription + (isFlags ? " [是标志位枚举]" : string.Empty);
                            enumDbDescription =
                                $@"( {(enumTypeDbDescription.IsNullOrWhiteSpace() ? "" : $@"{enumTypeDbDescription}; ")}{string.Join("; ", descList)} )";
                        }

                        migrationBuilder.AddOrUpdateColumnDescription(entityType.EntityType.GetTableName(),
                            property.GetColumnName(),
                            $@"{(entityType.ClrType?.GetProperty(property.Name)
                                    ?.GetCustomAttribute(typeof(DbDescriptionAttribute)) as DbDescriptionAttribute)
                                ?.Description}{(enumDbDescription.IsNullOrWhiteSpace() ? "" : $@" {enumDbDescription}")}"
                            , entityType.EntityType.GetSchema().IsNullOrEmpty() ? defaultSchema : entityType.EntityType.GetSchema());
                    }
                }
            }
        }

        /// <summary>
        /// 从模型注解添加表和列说明，需要先在OnModelCreating方法调用ConfigDatabaseDescription生成注解
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="migration"></param>
        /// <returns></returns>
        public static MigrationBuilder ApplyDatabaseDescription(this MigrationBuilder migrationBuilder, Migration migration)
        {
            var defaultSchema = "dbo";
            var descriptionAnnotationName = ModelBuilderExtensions.DbDescriptionAnnotationName;

            foreach (var entityType in migration.TargetModel.GetEntityTypes())
            {
                //添加表说明
                var tableName = entityType.GetTableName();
                var schema = entityType.GetSchema();
                var tableDescriptionAnnotation = entityType.FindAnnotation(descriptionAnnotationName);

                if (tableDescriptionAnnotation != null)
                {
                    migrationBuilder.AddOrUpdateTableDescription(
                        tableName,
                        tableDescriptionAnnotation.Value.ToString(),
                        schema.IsNullOrEmpty() ? defaultSchema : schema);
                }

                //添加列说明
                foreach (var property in entityType.GetProperties())
                {
                    var columnDescriptionAnnotation = property.FindAnnotation(descriptionAnnotationName);

                    if (columnDescriptionAnnotation != null)
                    {
                        migrationBuilder.AddOrUpdateColumnDescription(
                            tableName,
                            property.GetColumnName(),
                            columnDescriptionAnnotation.Value.ToString(),
                            schema.IsNullOrEmpty() ? defaultSchema : schema);
                    }
                }
            }

            return migrationBuilder;
        }

        /// <summary>
        /// 创建树形实体视图
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">视图对应的表名</param>
        /// <param name="columns">视图列集合</param>
        public static MigrationBuilder CreateTreeEntityView(this MigrationBuilder migrationBuilder, string tableName, IEnumerable<string> columns)
        {
            columns = columns.Select(c => $"[{c}]");
            var childColumns = columns.Select(c => $@"child.{c}");
            migrationBuilder.Sql(MigrationSqlTemplate.TreeEntityViewCreateSqlTemplate
                .Replace("{tableName}", tableName)
                .Replace("{columns}", string.Join(", ", columns))
                .Replace("{child.columns}", string.Join(", ", childColumns))
            );

            return migrationBuilder;
        }

        /// <summary>
        /// 自动扫描迁移模型并创建树形实体视图
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="migration">迁移类实例</param>
        /// <param name="viewAssembly">实体类所在程序集</param>
        public static MigrationBuilder CreateTreeEntityView(this MigrationBuilder migrationBuilder, IEnumerable<IEntityType> entities)
        {
            foreach (var entityType in entities.Where(x => x.FindAnnotation("IsTreeEntity") != null))
            {
                migrationBuilder.CreateTreeEntityView(entityType.GetTableName(),
                    entityType.GetProperties().Select(pro => pro.GetColumnName()));
            }

            return migrationBuilder;
        }

        /// <summary>
        /// 创建Identity实体树形实体视图
        /// （Identity必须从Microsoft.AspNetCore.Identity的类型继承
        /// 无法用DomainTreeEntityBase&lt;,,&gt;类型类扫描
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="migration">迁移类实例</param>
        /// <param name="viewAssembly">实体类所在程序集</param>
        /// <returns></returns>
        //public static MigrationBuilder CreateIdentityTreeEntityView(this MigrationBuilder migrationBuilder, Migration migration,
        //    Assembly viewAssembly)
        //{
        //    if (viewAssembly == null)
        //        throw new NullReferenceException($"{nameof(viewAssembly)} cannot be null.");

        //    var entityType = migration.TargetModel.GetEntityTypes().Single(entity =>
        //        viewAssembly.GetType(entity.Name).IsDerivedFrom(typeof(ApplicationRole)));
        //    if (entityType != null)
        //    {
        //        migrationBuilder.CreateTreeEntityView(entityType.Relational().TableName,
        //            entityType.GetProperties().Select(pro => pro.Relational().ColumnName));
        //    }

        //    return migrationBuilder;
        //}

        /// <summary>
        /// 删除树形实体视图
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">视图对应的表名</param>
        public static MigrationBuilder DropTreeEntityView(this MigrationBuilder migrationBuilder, string tableName)
        {
            migrationBuilder.Sql(MigrationSqlTemplate.TreeEntityViewDropSqlTemplate.Replace("{tableName}", tableName));

            return migrationBuilder;
        }
    }
}
