cmd 初始化迁移命令，在 IdentityServer 项目文件夹中执行 cmd
```
dotnet ef migrations add InitialApplicationDbMigration -c ApplicationDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o Application
dotnet ef migrations add InitialApplicationDbMigration -c ApplicationIdentityDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o Identity
dotnet ef migrations add InitialApplicationDbMigration -c ApplicationPermissionDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o Permission

dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c IdentityServerConfigurationDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o IdentityServer/ConfigurationDb
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c IdentityServerPersistedGrantDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o IdentityServer/PersistedGrantDb

dotnet ef migrations add InitialAdminAuditLogDbMigration -c AdminAuditLogDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o IdentityServer/AdminAuditLogDb
dotnet ef migrations add InitialAdminLogDbMigration -c AdminLogDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o IdentityServer/AdminLogDb

dotnet ef migrations add InitialLocalizationDbMigration -c LocalizationModelContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o Application/LocalizationDb
```

vs2019 初始化迁移命令，在包管理控制台
```
Add-Migration InitialApplicationDbMigration -Context ApplicationDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Application
Add-Migration InitialApplicationIdentityDbMigration -Context ApplicationIdentityDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Identity
Add-Migration InitialApplicationPermissionDbMigration -Context ApplicationPermissionDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Permission

Add-Migration InitialIdentityServerConfigurationDbMigration -Context IdentityServerConfigurationDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir IdentityServer/ConfigurationDb
Add-Migration InitialIdentityServerPersistedGrantDbMigration -Context IdentityServerPersistedGrantDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir IdentityServer/PersistedGrantDb

Add-Migration InitialAdminAuditLogDbMigration -Context AdminAuditLogDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir IdentityServer/AdminAuditLogDb
Add-Migration InitialAdminLogDbMigration -Context AdminLogDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir IdentityServer/AdminLogDb

Add-Migration InitialLocalizationDbMigration -Context LocalizationModelContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Application/LocalizationDb
```

在迁移中自动创建视图（仅限 MS SqlServer）
<br>别忘了在 `protected override void OnModelCreating(ModelBuilder builder)`中调用`builder.ConfigDatabaseDescription();`
```
    //Up的结尾处
    //自动扫描迁移模型并创建树形实体视图
    migrationBuilder.ApplyDatabaseDescription(this);
    migrationBuilder.CreateTreeEntityView(TargetModel.GetEntityTypes());
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    //Down的开始处
    //删除树形实体视图，这个没办法自动扫描
    migrationBuilder.DropTreeEntityView("AppRoles")
        .DropTreeEntityView("TreeDomains")
    .DropTreeEntityView("Organizations")
    .DropTreeEntityView("Menus");

    Add-Migration InitialTestDbMigration -Context TestDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Test
```