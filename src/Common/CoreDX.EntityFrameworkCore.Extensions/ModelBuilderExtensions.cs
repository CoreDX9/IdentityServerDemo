using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreDX.Common.Util.TypeExtensions;
using CoreDX.Domain.Core.Entity;
using CoreDX.EntityFrameworkCore.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoreDX.EntityFrameworkCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// [UniqueKey(groupId: "1", order: 0)]
        /// public string ColumnName { get; set; }
        ///
        /// [UniqueKey(groupId: "1", order: 1)]
        /// public string ColumnName2 { get; set; }
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static ModelBuilder ConfigUniqueKey(this ModelBuilder modelBuilder)
        {
            // Iterate through all EF Entity types
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                #region Convert UniqueKeyAttribute on Entities to UniqueKey in DB

                var properties = entityType.GetProperties();
                if ((properties != null) && (properties.Any()))
                {
                    foreach (var property in properties)
                    {
                        foreach (var uniqueKey in GetUniqueKeyAttributes(entityType, property).Where(x => x.Order == 0))
                        {
                            // Single column Unique Key
                            if (uniqueKey.GroupId.IsNullOrWhiteSpace())
                            {
                                entityType.AddIndex(property).IsUnique = true;
                            }
                            // Multiple column Unique Key
                            else
                            {
                                var mutableProperties = new List<IMutableProperty>();
                                properties.ToList().ForEach(x =>
                                {
                                    var uks = GetUniqueKeyAttributes(entityType, x);
                                    if (uks != null)
                                    {
                                        foreach (var uk in uks)
                                        {
                                            if ((uk != null) && (uk.GroupId == uniqueKey.GroupId))
                                            {
                                                mutableProperties.Add(x);
                                            }
                                        }
                                    }
                                });
                                entityType.AddIndex(mutableProperties).IsUnique = true;
                            }
                        }
                    }
                }

                #endregion Convert UniqueKeyAttribute on Entities to UniqueKey in DB
            }

            return modelBuilder;
        }

        /// <summary>
        /// 获取唯一键特性集合
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="property">实体属性</param>
        /// <returns>唯一键特性集合</returns>
        private static IEnumerable<UniqueKeyAttribute> GetUniqueKeyAttributes(IMutableEntityType entityType, IMutableProperty property)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if (entityType.ClrType == null)
            {
                throw new NullReferenceException(nameof(entityType.ClrType));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (property.Name == null)
            {
                throw new NullReferenceException(nameof(property.Name));
            }

            var propInfo = entityType.ClrType.GetProperty(
                property.Name,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);

            if (propInfo == null)
            {
                return new UniqueKeyAttribute[0];
            }

            return propInfo.GetCustomAttributes<UniqueKeyAttribute>();
        }

        public const string DbDescriptionAnnotationName = "DbDescription";

        /// <summary>
        /// 配置数据库表和列说明
        /// </summary>
        /// <param name="modelBuilder">模型构造器</param>
        /// <returns>模型构造器</returns>
        public static ModelBuilder ConfigDatabaseDescription(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                //添加表说明
                if (entityType.FindAnnotation(DbDescriptionAnnotationName) == null && entityType.ClrType?.CustomAttributes.Any(
                        attr => attr.AttributeType == typeof(DbDescriptionAttribute)) == true)
                {
                    entityType.AddAnnotation(DbDescriptionAnnotationName,
                        (entityType.ClrType.GetCustomAttribute(typeof(DbDescriptionAttribute)) as DbDescriptionAttribute
                        )?.Description);
                }

                //添加列说明
                foreach (var property in entityType.GetProperties())
                {
                    if (property.FindAnnotation(DbDescriptionAnnotationName) == null && property.PropertyInfo?.CustomAttributes
                            .Any(attr => attr.AttributeType == typeof(DbDescriptionAttribute)) == true)
                    {
                        var propertyInfo = property.PropertyInfo;
                        var propertyType = propertyInfo?.PropertyType;
                        //如果该列的实体属性是枚举类型，把枚举的说明追加到列说明
                        var enumDbDescription = string.Empty;
                        if (propertyType.IsEnum
                            || (propertyType.IsDerivedFrom(typeof(Nullable<>)) && propertyType.GenericTypeArguments[0].IsEnum))
                        {
                            var @enum = propertyType.IsDerivedFrom(typeof(Nullable<>))
                                ? propertyType.GenericTypeArguments[0]
                                : propertyType;

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

                        property.AddAnnotation(DbDescriptionAnnotationName,
                            $@"{(propertyInfo.GetCustomAttribute(typeof(DbDescriptionAttribute)) as DbDescriptionAttribute)
                                ?.Description}{(enumDbDescription.IsNullOrWhiteSpace() ? "" : $@" {enumDbDescription}")}");
                    }
                }
            }

            return modelBuilder;
        }

        /// <summary>
        /// 配置数据库表和列说明2（使用 EF Core 3的新增 API SetComment()配置，估计可以免除在迁移中手动调用执行sql的麻烦，等待测试效果
        /// 结果证明并没有卵用。 3.0.1 。还会导致迁移工具bug）
        /// </summary>
        /// <param name="modelBuilder">模型构造器</param>
        /// <returns>模型构造器</returns>
        public static ModelBuilder ConfigDatabaseDescription2(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                //添加表说明
                if (entityType.FindAnnotation(DbDescriptionAnnotationName) == null && entityType.ClrType?.CustomAttributes.Any(
                        attr => attr.AttributeType == typeof(DbDescriptionAttribute)) == true)
                {
                    entityType.SetComment((entityType.ClrType.GetCustomAttribute(typeof(DbDescriptionAttribute)) as DbDescriptionAttribute)?.Description);
                }

                //添加列说明
                foreach (var property in entityType.GetProperties())
                {
                    if (property.FindAnnotation(DbDescriptionAnnotationName) == null && property.PropertyInfo?.CustomAttributes
                            .Any(attr => attr.AttributeType == typeof(DbDescriptionAttribute)) == true)
                    {
                        var propertyInfo = property.PropertyInfo;
                        var propertyType = propertyInfo?.PropertyType;
                        //如果该列的实体属性是枚举类型，把枚举的说明追加到列说明
                        var enumDbDescription = string.Empty;
                        if (propertyType.IsEnum
                            || (propertyType.IsDerivedFrom(typeof(Nullable<>)) && propertyType.GenericTypeArguments[0].IsEnum))
                        {
                            var @enum = propertyType.IsDerivedFrom(typeof(Nullable<>))
                                ? propertyType.GenericTypeArguments[0]
                                : propertyType;

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

                        property.SetComment($@"{(propertyInfo.GetCustomAttribute(typeof(DbDescriptionAttribute)) as DbDescriptionAttribute)
                                ?.Description}{(enumDbDescription.IsNullOrWhiteSpace() ? "" : $@" {enumDbDescription}")}");
                    }
                }
            }

            return modelBuilder;
        }

        /// <summary>
        /// 配置主键Guid转字符串转换器（为不支持存储Guid的数据库使用）
        /// </summary>
        /// <param name="modelBuilder">模型构造器</param>
        /// <returns>模型构造器</returns>
        public static ModelBuilder ConfigKeyGuidToStringConverter(this ModelBuilder modelBuilder)
        {
            foreach (var domain in modelBuilder.Model.GetEntityTypes().Where(e =>
                    !e.IsKeyless && !e.IsOwned() && e.ClrType.IsDerivedFrom(typeof(IEntity<Guid>)))
                .Select(e => e.ClrType))
            {
                modelBuilder.Entity(domain, b =>
                {
                    b.Property<Guid>(nameof(IEntity<Guid>.Id)).HasConversion(new GuidToStringConverter());
                });
            }

            return modelBuilder;
        }

        /// <summary>
        /// 配置模型中所有Guid属性的Guid转字符串转换器（为不支持存储Guid的数据库使用）
        /// </summary>
        /// <param name="modelBuilder">模型构造器</param>
        /// <returns>模型构造器</returns>
        public static ModelBuilder ConfigPropertiesGuidToStringConverter(this ModelBuilder modelBuilder)
        {
            foreach (var domain in modelBuilder.Model.GetEntityTypes().Select(e => e.ClrType))
            {
                modelBuilder.Entity(domain, b =>
                {
                    foreach(var property in b.Metadata.GetProperties())
                    {
                        if(property.ClrType == typeof(Guid))
                        {
                            property.SetValueConverter(new GuidToStringConverter());
                        }
                    }
                });
            }

            return modelBuilder;
        }
    }
}
