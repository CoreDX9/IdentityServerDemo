using System;
using CoreDX.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDX.Application.DbMigration.Application
{
    public partial class InitialApplicationDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreatorId = table.Column<int>(nullable: true),
                    LastModifierId = table.Column<int>(nullable: true),
                    SC = table.Column<string>(nullable: true),
                    ComplexProperty_C1 = table.Column<string>(nullable: true),
                    ComplexProperty_C2 = table.Column<string>(nullable: true),
                    ComplexProperty_ComplexProperty2_C3 = table.Column<string>(nullable: true),
                    ComplexProperty_ComplexProperty2_C4 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    CreatorId = table.Column<int>(nullable: true),
                    LastModifierId = table.Column<int>(nullable: true),
                    MenuIcon_Type = table.Column<string>(nullable: true),
                    MenuIcon_Value = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Order = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreeDomains",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    CreatorId = table.Column<int>(nullable: true),
                    LastModifierId = table.Column<int>(nullable: true),
                    SampleColumn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeDomains_TreeDomains_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TreeDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreatorId = table.Column<int>(nullable: true),
                    LastModifierId = table.Column<int>(nullable: true),
                    MenuItemIcon_Type = table.Column<string>(nullable: true),
                    MenuItemIcon_Value = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    Order = table.Column<short>(nullable: false),
                    MenuId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_MenuId",
                table: "MenuItems",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                table: "Menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeDomains_ParentId",
                table: "TreeDomains",
                column: "ParentId");

            migrationBuilder.ApplyDatabaseDescription(this);
            migrationBuilder.CreateTreeEntityView(TargetModel.GetEntityTypes());

            #region 创建 MiniProfiler 的分析数据存储表

            var sqls = new StackExchange.Profiling.Storage.SqlServerStorage("").TableCreationScripts;
            foreach (var sql in sqls)
            {
                migrationBuilder.Sql(sql);
            }

            #endregion
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            #region 删除 MiniProfiler 的分析数据存储表

            migrationBuilder.DropTable(
                name: "MiniProfilerClientTimings");

            migrationBuilder.DropTable(
                name: "MiniProfilerTimings");

            migrationBuilder.DropTable(
                name: "MiniProfilers");

            #endregion

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "TreeDomains");

            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}
