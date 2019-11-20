namespace CoreDX.EntityFrameworkCore.Extensions.SqlServer
{
    /// <summary>
    /// 数据迁移扩展Sql模板
    /// </summary>
    public static class MigrationSqlTemplate
    {
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

        #region 树形实体视图sql模板

        /// <summary>
        /// 树形实体视图创建Sql模板
        /// </summary>
        public const string TreeEntityViewCreateSqlTemplate = TreeEntityViewDropSqlTemplate + "\r\n" + @"
create view view_tree_{tableName}    --创建视图
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
if exists(select * from sysobjects where id=OBJECT_ID(N'view_tree_{tableName}') and objectproperty(id,N'IsView')=1)
drop view view_tree_{tableName}    --删除视图
go";

        #endregion 树形实体视图sql
    }
}
