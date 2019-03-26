using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbMigration.Identity
{
    public partial class IdentityDbMigration_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserOrganizations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserOrganizations_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationUserOrganizations_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationUserOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserOrganizations_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserOrganizations_CreationUserId",
                table: "ApplicationUserOrganizations",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserOrganizations_LastModificationUserId",
                table: "ApplicationUserOrganizations",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserOrganizations_OrganizationId",
                table: "ApplicationUserOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserOrganizations_UserId",
                table: "ApplicationUserOrganizations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserOrganizations");
        }
    }
}
