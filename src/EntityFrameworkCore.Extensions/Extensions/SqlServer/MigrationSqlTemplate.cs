namespace EntityFrameworkCore.Extensions.Extensions.SqlServer
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
    }
}
