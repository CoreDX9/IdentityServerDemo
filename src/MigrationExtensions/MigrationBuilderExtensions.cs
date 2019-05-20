using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MigrationExtensions.DataAnnotations;
using Util.Extensions;

namespace MigrationExtensions
{
    /// <summary>
    /// 数据迁移扩展
    /// </summary>
    public static class MigrationBuilderExtensions
    {
        #region 树形实体视图sql模板

        /// <summary>
        /// 树形实体视图创建Sql模板
        /// </summary>
        public const string TreeEntityViewCreateSqlTemplate = @"
--判断视图是否存在
if exists(select * from sysobjects where id=OBJECT_ID(N'view_{tableName}') and objectproperty(id,N'IsView')=1)
drop view view_{tableName}    --删除视图
go
create view view_{tableName}    --创建视图
--with schemaBinding    --如果要创建带索引的视图要加上这句
as
with temp({columns}, Depth, Path, HasChildren) as
(
	--初始查询（这里的 ParentId is null 在我的数据中是最底层的根节点）
	select {columns},
		0 as Depth,
		cast(Id as nvarchar(4000)) as Path, --如果Id使用Guid类型，可能会导致层数太深时出问题（大概100层左右，超过4000字之后的字符串会被砍掉），Guid的字数太多了
		HasChildren = (case when exists(select 1 from {tableName} where {tableName}.ParentId = root.id) then cast(1 as bit) else cast(0 as bit) end)
	from {tableName} as root
	where ParentId is null

	union all
	--递归条件
	select {child.columns},
		parent.Depth+1,
		parent.Path + '/' + cast(child.Id as nvarchar(4000)),
		HasChildren = (case when exists(select 1 from {tableName} where {tableName}.ParentId = child.id) then cast(1 as bit) else cast(0 as bit) end)
	from {tableName} as child --3：这里的临时表和原始数据表都必须使用别名不然递归的时候不知道查询的是那个表的列
	inner join
	temp as parent
	on (child.ParentId = parent.Id) --这个关联关系很重要，一定要理解一下谁是谁的父节点
)
select * --top 100 percent * --Id, pId, Str, depth, path, hasChild -- 要创建索引的视图不能使用 select * 的写法 -- 带公用表表达式的视图无法创建索引
from temp
--order by temp.Id --4：递归完成后 一定不要少了这句查询语句 否则会报错 -- 创建视图则无需排序，视图的排序对外部引用无效，要在外部查询指定排序
go

----在视图上建立唯一的聚集索引
--create unique clustered index
--Index_View on view_{tableName}(Id)

----在视图上建立非聚集索引
--create index
--Index_View_depth on view_{tableName}(depth)

--create index
--Index_View_hasChild on view_{tableName}(hasChild)

--create index
--Index_View_path on view_{tableName}(path)
--go

----为表明已经给视图建立一个索引，并且它确实占用数据库的空间，运行下面的脚本查明聚集索引有多少行以及视图占用多少空间。
--execute sp_spaceused 'viewTree'
--go";

        /// <summary>
        /// 树形实体视图删除Sql模板
        /// </summary>
        public const string TreeEntityViewDropSqlTemplate = @"
--判断视图是否存在
if exists(select * from sysobjects where id=OBJECT_ID(N'view_{tableName}') and objectproperty(id,N'IsView')=1)
drop view view_{tableName}    --删除视图
go";

        #endregion 树形实体视图sql

        #region 表、列说明sql模板

        public const string AddTableDbDescriptionTemplate = @"
if exists (
	select t.name as tname, d.value as Description
	from sysobjects t
	left join sys.extended_properties d
	on t.id = d.major_id and d.minor_id = 0 and d.name = 'MS_Description'
	where t.name = '{tableName}' and d.value is not null)
begin
	exec sys.sp_dropextendedproperty
    @name=N'MS_Description'
  , @level0type=N'SCHEMA'
  , @level0name=N'{schema}'
  , @level1type=N'TABLE'
  , @level1name=N'{tableName}'
  , @level2type=NULL
  , @level2name=NULL
end
go

exec sys.sp_addextendedproperty
    @name=N'MS_Description'
  , @value=N'{tableDescription}'
  , @level0type=N'SCHEMA'
  , @level0name=N'{schema}'
  , @level1type=N'TABLE'
  , @level1name=N'{tableName}'
  , @level2type= NULL
  , @level2name= NULL
go";

        public const string DropTableDbDescriptionTemplate = @"
if exists (
	select t.name as tname, d.value as Description
	from sysobjects t
	left join sys.extended_properties d
	on t.id = d.major_id and d.minor_id = 0 and d.name = 'MS_Description'
	where t.name = '{tableName}' and d.value is not null)
begin
	exec sys.sp_dropextendedproperty
    @name=N'MS_Description'
  , @level0type=N'SCHEMA'
  , @level0name=N'{schema}'
  , @level1type=N'TABLE'
  , @level1name=N'{tableName}'
  , @level2type=NULL
  , @level2name=NULL
end
go";

        public const string AddColumnDbDescriptionTemplate = @"
if exists (
	select t.name as tname,c.name as cname, d.value as Description
	from sysobjects t
	left join syscolumns c
	on c.id=t.id and t.xtype='U' and t.name<>'dtproperties'
	left join sys.extended_properties d
	on c.id=d.major_id and c.colid=d.minor_id and d.name = 'MS_Description'
	where t.name = '{tableName}' and c.name = '{columnName}' and d.value is not null)
begin
	exec sys.sp_dropextendedproperty
    @name=N'MS_Description'
  , @level0type=N'SCHEMA'
  , @level0name=N'{schema}'
  , @level1type=N'TABLE'
  , @level1name=N'{tableName}'
  , @level2type=N'COLUMN'
  , @level2name=N'{columnName}'
end
go

exec sys.sp_addextendedproperty
    @name=N'MS_Description'
  , @value=N'{columnDescription}'
  , @level0type=N'SCHEMA'
  , @level0name=N'{schema}'
  , @level1type=N'TABLE'
  , @level1name=N'{tableName}'
  , @level2type=N'COLUMN'
  , @level2name=N'{columnName}'
go";

        public const string DropColumnDbDescriptionTemplate = @"
if exists (
	select t.name as tname,c.name as cname, d.value as Description
	from sysobjects t
	left join syscolumns c
	on c.id=t.id and t.xtype='U' and t.name<>'dtproperties'
	left join sys.extended_properties d
	on c.id=d.major_id and c.colid=d.minor_id and d.name = 'MS_Description'
	where t.name = '{tableName}' and c.name = '{columnName}' and d.value is not null)
begin
	exec sys.sp_dropextendedproperty
    @name=N'MS_Description'
  , @level0type=N'SCHEMA'
  , @level0name=N'{schema}'
  , @level1type=N'TABLE'
  , @level1name=N'{tableName}'
  , @level2type=N'COLUMN'
  , @level2name=N'{columnName}'
end
go";

        #endregion

        /// <summary>
        /// 创建树形实体视图
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">视图对应的表名</param>
        /// <param name="columns">视图列集合</param>
        public static void CreateTreeEntityView(this MigrationBuilder migrationBuilder, string tableName, IEnumerable<string> columns)
        {
            var childColumns = columns.Select(c => $@"child.{c}");
            migrationBuilder.Sql(TreeEntityViewCreateSqlTemplate
                .Replace("{tableName}", tableName)
                .Replace("{columns}", string.Join(", ", columns))
                .Replace("{child.columns}", string.Join(", ", childColumns))
            );
        }

        /// <summary>
        /// 自动扫描迁移模型并创建树形实体视图
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="migration">迁移类实例</param>
        /// <param name="entityAssembly">实体类所在程序集</param>
        public static void CreateTreeEntityView(this MigrationBuilder migrationBuilder, Migration migration,
            Assembly entityAssembly)
        {
            if (entityAssembly == null)
                throw new NullReferenceException($"{nameof(entityAssembly)} cannot be null.");

            foreach (var entityType in migration.TargetModel.GetEntityTypes().Where(entity =>
                entityAssembly.GetType(entity.Name).HasImplementedRawGeneric(typeof(ITree<>))))
            {
                migrationBuilder.CreateTreeEntityView(entityType.Relational().TableName,
                    entityType.GetProperties().Select(pro => pro.Relational().ColumnName));
            }
        }

        /// <summary>
        /// 删除树形实体视图
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">视图对应的表名</param>
        public static void DropTreeEntityView(this MigrationBuilder migrationBuilder, string tableName)
        {
            migrationBuilder.Sql(TreeEntityViewDropSqlTemplate.Replace("{tableName}", tableName));
        }

        /// <summary>
        /// 添加表说明
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">表名</param>
        /// <param name="description">说明</param>
        /// <param name="schema">架构</param>
        public static void AddTableDescription(this MigrationBuilder migrationBuilder, string tableName,
            string description, string schema = "dbo")
        {
            if (!description.IsNullOrWhiteSpace() && description.Contains('\'')) description = description.Replace("'", "''");
            migrationBuilder.Sql(AddTableDbDescriptionTemplate
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
            migrationBuilder.Sql(DropTableDbDescriptionTemplate
                .Replace("{schema}", schema)
                .Replace("{tableName}", tableName));
        }

        /// <summary>
        /// 添加列说明
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <param name="description">说明</param>
        /// <param name="schema">架构</param>
        public static void AddColumnDescription(this MigrationBuilder migrationBuilder, string tableName,
            string columnName, string description, string schema = "dbo")
        {
            if (!description.IsNullOrWhiteSpace() && description.Contains('\'')) description = description.Replace("'", "''");
            migrationBuilder.Sql(AddColumnDbDescriptionTemplate
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
            migrationBuilder.Sql(DropColumnDbDescriptionTemplate
                .Replace("{schema}", schema)
                .Replace("{tableName}", tableName)
                .Replace("{columnName}", columnName));
        }

        /// <summary>
        /// 自动扫描迁移模型并添加表和列说明
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="migration">迁移类实例</param>
        /// <param name="entityAssembly">实体类所在程序集</param>
        /// <param name="schema">架构</param>
        public static void AddDatabaseDescription(this MigrationBuilder migrationBuilder, Migration migration,
            Assembly entityAssembly)
        {
            if (entityAssembly == null)
                throw new NullReferenceException($"{nameof(entityAssembly)} cannot be null.");

            foreach (var entityType in migration.TargetModel.GetEntityTypes().Select(entity =>
                new {EntityType = entity, ClrType = entityAssembly.GetType(entity.Name)}))
            {
                //添加表说明
                if (entityType.ClrType?.CustomAttributes.Any(
                        attr => attr.AttributeType == typeof(DbDescriptionAttribute)) == true)
                {
                    migrationBuilder.AddTableDescription(entityType.EntityType.Relational().TableName,
                        (entityType.ClrType.GetCustomAttribute(typeof(DbDescriptionAttribute)) as DbDescriptionAttribute
                        )?.Description, entityType.EntityType.Relational().Schema);
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
                                    .HasImplementedRawGeneric(typeof(Nullable<>)) == true
                                && entityType.ClrType?.GetProperty(property.Name)?.PropertyType.GenericTypeArguments[0]
                                    .IsEnum == true))
                        {
                            var @enum = entityType.ClrType?.GetProperty(property.Name)?.PropertyType
                                            .HasImplementedRawGeneric(typeof(Nullable<>)) == true
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

                            var enumTypeDbDescription =
                                (@enum?.GetCustomAttributes(typeof(DbDescriptionAttribute), false).FirstOrDefault() as
                                    DbDescriptionAttribute)?.Description;
                            enumDbDescription =
                                $@"( {(enumTypeDbDescription.IsNullOrWhiteSpace() ? "" : $@"{enumTypeDbDescription}; ")}{string.Join("; ", descList)} )";
                        }

                        migrationBuilder.AddColumnDescription(entityType.EntityType.Relational().TableName,
                            property.Relational().ColumnName,
                            $@"{(entityType.ClrType?.GetProperty(property.Name)
                                    ?.GetCustomAttribute(typeof(DbDescriptionAttribute)) as DbDescriptionAttribute)
                                ?.Description}{(enumDbDescription.IsNullOrWhiteSpace() ? "" : $@" {enumDbDescription}")}"
                            , entityType.EntityType.Relational().Schema);
                    }
                }
            }
        }
    }
}
