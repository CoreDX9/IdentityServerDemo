using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreDX.Common.Util.TypeExtensions;
using CoreDX.Domain.Core.Entity;
using CoreDX.Domain.Model.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDX.Application.EntityFrameworkCore.Extensions
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
		'/' + cast(Id as nvarchar(max)) as Path, --如果Id使用Guid类型，可能会导致层数太深时出问题（大概100层左右，超过4000字之后的字符串会被砍掉,sqlserver 2005以后用 nvarchar(max)可以突破限制），Guid的字数太多了
		HasChildren = (case when exists(select 1 from {tableName} where {tableName}.ParentId = root.id) then cast(1 as bit) else cast(0 as bit) end)
	from {tableName} as root
	where ParentId is null

	union all
	--递归条件
	select {child.columns},
		parent.Depth+1,
		parent.Path + '/' + cast(child.Id as nvarchar(max)),
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
            migrationBuilder.Sql(TreeEntityViewCreateSqlTemplate
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
        public static MigrationBuilder CreateTreeEntityView(this MigrationBuilder migrationBuilder, Migration migration,
            Assembly viewAssembly)
        {
            if (viewAssembly == null)
                throw new NullReferenceException($"{nameof(viewAssembly)} cannot be null.");

            foreach (var entityType in migration.TargetModel.GetEntityTypes().Where(entity =>
                viewAssembly.GetType(entity.Name).IsDerivedFrom(typeof(IDomainTreeEntity<,>))))
            {
                migrationBuilder.CreateTreeEntityView(entityType.Relational().TableName,
                    entityType.GetProperties().Select(pro => pro.Relational().ColumnName));
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
        public static MigrationBuilder CreateIdentityTreeEntityView(this MigrationBuilder migrationBuilder, Migration migration,
            Assembly viewAssembly)
        {
            if (viewAssembly == null)
                throw new NullReferenceException($"{nameof(viewAssembly)} cannot be null.");

            var entityType = migration.TargetModel.GetEntityTypes().Single(entity =>
                viewAssembly.GetType(entity.Name).IsDerivedFrom(typeof(ApplicationRole)));
            if (entityType != null)
            {
                migrationBuilder.CreateTreeEntityView(entityType.Relational().TableName,
                    entityType.GetProperties().Select(pro => pro.Relational().ColumnName));
            }

            return migrationBuilder;
        }

        /// <summary>
        /// 删除树形实体视图
        /// </summary>
        /// <param name="migrationBuilder">迁移构造器</param>
        /// <param name="tableName">视图对应的表名</param>
        public static MigrationBuilder DropTreeEntityView(this MigrationBuilder migrationBuilder, string tableName)
        {
            migrationBuilder.Sql(TreeEntityViewDropSqlTemplate.Replace("{tableName}", tableName));

            return migrationBuilder;
        }
    }
}
