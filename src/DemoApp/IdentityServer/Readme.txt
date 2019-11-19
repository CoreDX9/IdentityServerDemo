cmd初始化迁移命令,在IdentityServer项目文件夹中执行cmd
dotnet ef migrations add InitialApplicationDbMigration -c ApplicationDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o Application
dotnet ef migrations add InitialApplicationDbMigration -c ApplicationIdentityDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o Identity
dotnet ef migrations add InitialApplicationDbMigration -c ApplicationPermissionDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o Permission

dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o IdentityServer/ConfigurationDb

dotnet ef migrations add InitialLocalizationDbMigration -c LocalizationModelContext -p "../../Infrastructure/CoreDX.Application.DbMigration" -o Application/LocalizationDb

vs2019初始化迁移命令，在包管理控制台
Add-Migration InitialApplicationDbMigration -Context ApplicationDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Application
Add-Migration InitialApplicationIdentityDbMigration -Context ApplicationIdentityDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Identity
Add-Migration InitialApplicationPermissionDbMigration -Context ApplicationPermissionDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Permission

Add-Migration InitialIdentityServerPersistedGrantDbMigration -Context PersistedGrantDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir IdentityServer/PersistedGrantDb
Add-Migration InitialIdentityServerConfigurationDbMigration -Context ConfigurationDbContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir IdentityServer/ConfigurationDb

Add-Migration InitialLocalizationDbMigration -Context LocalizationModelContext -Project CoreDX.Application.DbMigration -StartupProject IdentityServer -OutputDir Application/LocalizationDb

            //自动扫描迁移模型并创建树形实体视图
            migrationBuilder.CreateTreeEntityView(this,
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("Domain")))
				.CreateIdentityTreeEntityView(this, AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("Domain")))
                //从模型注解应用表和列说明
                .ApplyDatabaseDescription(this);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //删除树形实体视图，这个没办法自动扫描
            migrationBuilder.DropTreeEntityView("AppRoles")
                .DropTreeEntityView("TreeDomains")
				.DropTreeEntityView("Organizations")
				.DropTreeEntityView("Menus");

