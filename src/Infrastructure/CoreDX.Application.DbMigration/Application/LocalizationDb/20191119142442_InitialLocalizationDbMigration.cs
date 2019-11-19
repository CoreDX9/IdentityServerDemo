using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDX.Application.DbMigration.Application.LocalizationDb
{
    public partial class InitialLocalizationDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExportHistoryDbSet",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Exported = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportHistoryDbSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportHistoryDbSet",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imported = table.Column<DateTime>(nullable: false),
                    Information = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportHistoryDbSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationRecords",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    LocalizationCulture = table.Column<string>(nullable: false),
                    ResourceKey = table.Column<string>(nullable: false),
                    UpdatedTimestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationRecords", x => x.Id);
                    table.UniqueConstraint("AK_LocalizationRecords_Key_LocalizationCulture_ResourceKey", x => new { x.Key, x.LocalizationCulture, x.ResourceKey });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportHistoryDbSet");

            migrationBuilder.DropTable(
                name: "ImportHistoryDbSet");

            migrationBuilder.DropTable(
                name: "LocalizationRecords");
        }
    }
}
