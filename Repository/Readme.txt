初始化迁移命令,在IdentityServer项目文件夹中执行cmd
dotnet ef migrations add InitialIdentityDbMigration -c ApplicationIdentityDbContext -p ../DbMigration -o Identity

dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -p ../DbMigration -o IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -p ../DbMigration -o IdentityServer/ConfigurationDb

            //自动扫描迁移模型并创建树形实体视图
            migrationBuilder.CreateTreeEntityView(this,
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("Domain")))
                //从模型注解应用表和列说明
                .ApplyDatabaseDescription(this);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //删除树形实体视图，这个没办法自动扫描
            migrationBuilder.DropTreeEntityView("AppRoles")
                .DropTreeEntityView("TreeDomains");
