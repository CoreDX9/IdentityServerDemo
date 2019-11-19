using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDX.Application.DbMigration.Permission
{
    public partial class InitialApplicationPermissionDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAuthorizationRules",
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
                    Name = table.Column<string>(nullable: true),
                    AuthorizationRuleConfigJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAuthorizationRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionDefinitions",
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
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ValueType = table.Column<short>(nullable: false),
                    InsertOrder = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRequestAuthorizationRules",
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
                    HandlerMethodSignature = table.Column<string>(nullable: true),
                    TypeFullName = table.Column<string>(nullable: true),
                    IdentificationKey = table.Column<string>(nullable: true),
                    AuthorizationRuleId = table.Column<int>(nullable: true),
                    AuthorizationRuleId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRequestAuthorizationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRequestAuthorizationRules_AppAuthorizationRules_AuthorizationRuleId",
                        column: x => x.AuthorizationRuleId,
                        principalTable: "AppAuthorizationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppRequestAuthorizationRules_AppAuthorizationRules_AuthorizationRuleId1",
                        column: x => x.AuthorizationRuleId1,
                        principalTable: "AppAuthorizationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPermissionDeclarations",
                columns: table => new
                {
                    PermissionDefinitionId = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreatorId = table.Column<int>(nullable: true),
                    PermissionValue = table.Column<short>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPermissionDeclarations", x => new { x.OrganizationId, x.PermissionDefinitionId });
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissionDeclarations",
                columns: table => new
                {
                    PermissionDefinitionId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreatorId = table.Column<int>(nullable: true),
                    PermissionValue = table.Column<short>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissionDeclarations", x => new { x.RoleId, x.PermissionDefinitionId });
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionDeclarations",
                columns: table => new
                {
                    PermissionDefinitionId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreatorId = table.Column<int>(nullable: true),
                    PermissionValue = table.Column<short>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionDeclarations", x => new { x.UserId, x.PermissionDefinitionId });
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRequestAuthorizationRules_AuthorizationRuleId",
                table: "AppRequestAuthorizationRules",
                column: "AuthorizationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRequestAuthorizationRules_AuthorizationRuleId1",
                table: "AppRequestAuthorizationRules",
                column: "AuthorizationRuleId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclarations_PermissionDefinitionId",
                table: "OrganizationPermissionDeclarations",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclarations_PermissionDefinitionId",
                table: "RolePermissionDeclarations",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclarations_PermissionDefinitionId",
                table: "UserPermissionDeclarations",
                column: "PermissionDefinitionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppRequestAuthorizationRules");

            migrationBuilder.DropTable(
                name: "OrganizationPermissionDeclarations");

            migrationBuilder.DropTable(
                name: "RolePermissionDeclarations");

            migrationBuilder.DropTable(
                name: "UserPermissionDeclarations");

            migrationBuilder.DropTable(
                name: "AppAuthorizationRules");

            migrationBuilder.DropTable(
                name: "PermissionDefinitions");
        }
    }
}
