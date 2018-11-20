using System;
using System.Linq;
using EntityFrameworkCore.Extensions.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbMigration.Identity
{
    public partial class AddPermissionIdentityDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "UserLogins",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "TreeDomains",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "TreeDomains",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "Domains",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Domains",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppUserTokens",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppUsers",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppUserRoles",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppUserClaims",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppRoles",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppRoleClaims",
                nullable: false,
                defaultValueSql: "sysDateTimeOffset()",
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organization_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organization_Organization_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionDefinition",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ValueType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionDefinition_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermissionDefinition_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestAuthorizationRule",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    MethodSignName = table.Column<string>(nullable: true),
                    TypeFullName = table.Column<string>(nullable: true),
                    FriendlyName = table.Column<string>(nullable: true),
                    AdvanceAuthorizationRuleJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAuthorizationRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAuthorizationRule_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestAuthorizationRule_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPermissionDeclaration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    PermissionValue = table.Column<short>(nullable: false),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    ParentPermissionDeclarationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPermissionDeclaration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclaration_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclaration_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclaration_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclaration_OrganizationPermissionDeclaration_ParentPermissionDeclarationId",
                        column: x => x.ParentPermissionDeclarationId,
                        principalTable: "OrganizationPermissionDeclaration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissionDeclaration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    PermissionValue = table.Column<short>(nullable: false),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissionDeclaration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclaration_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclaration_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclaration_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionDeclaration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    PermissionValue = table.Column<short>(nullable: false),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionDeclaration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclaration_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclaration_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclaration_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestHandlerPermissionDeclaration",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    PermissionValue = table.Column<short>(nullable: false),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
                    RuleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHandlerPermissionDeclaration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclaration_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclaration_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclaration_RequestAuthorizationRule_RuleId",
                        column: x => x.RuleId,
                        principalTable: "RequestAuthorizationRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestHandlerPermissionDeclarationOrganization",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    PermissionDeclarationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHandlerPermissionDeclarationOrganization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationOrganization_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationOrganization_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationOrganization_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationOrganization_RequestHandlerPermissionDeclaration_PermissionDeclarationId",
                        column: x => x.PermissionDeclarationId,
                        principalTable: "RequestHandlerPermissionDeclaration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestHandlerPermissionDeclarationRole",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true),
                    PermissionDeclarationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHandlerPermissionDeclarationRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationRole_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationRole_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationRole_RequestHandlerPermissionDeclaration_PermissionDeclarationId",
                        column: x => x.PermissionDeclarationId,
                        principalTable: "RequestHandlerPermissionDeclaration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationRole_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organization_CreationUserId",
                table: "Organization",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_LastModificationUserId",
                table: "Organization",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_ParentId",
                table: "Organization",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclaration_CreationUserId",
                table: "OrganizationPermissionDeclaration",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclaration_LastModificationUserId",
                table: "OrganizationPermissionDeclaration",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclaration_OrganizationId",
                table: "OrganizationPermissionDeclaration",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclaration_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclaration",
                column: "ParentPermissionDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclaration_PermissionDefinitionId",
                table: "OrganizationPermissionDeclaration",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDefinition_CreationUserId",
                table: "PermissionDefinition",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDefinition_LastModificationUserId",
                table: "PermissionDefinition",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuthorizationRule_CreationUserId",
                table: "RequestAuthorizationRule",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuthorizationRule_LastModificationUserId",
                table: "RequestAuthorizationRule",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclaration_CreationUserId",
                table: "RequestHandlerPermissionDeclaration",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclaration_LastModificationUserId",
                table: "RequestHandlerPermissionDeclaration",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclaration_PermissionDefinitionId",
                table: "RequestHandlerPermissionDeclaration",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclaration_RuleId",
                table: "RequestHandlerPermissionDeclaration",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganization_CreationUserId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganization_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganization_OrganizationId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganization_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "PermissionDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationRole_CreationUserId",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationRole_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationRole_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "PermissionDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationRole_RoleId",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclaration_CreationUserId",
                table: "RolePermissionDeclaration",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclaration_LastModificationUserId",
                table: "RolePermissionDeclaration",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclaration_PermissionDefinitionId",
                table: "RolePermissionDeclaration",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclaration_RoleId",
                table: "RolePermissionDeclaration",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclaration_CreationUserId",
                table: "UserPermissionDeclaration",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclaration_LastModificationUserId",
                table: "UserPermissionDeclaration",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclaration_PermissionDefinitionId",
                table: "UserPermissionDeclaration",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclaration_UserId",
                table: "UserPermissionDeclaration",
                column: "UserId");

            migrationBuilder.ApplyDatabaseDescription(this,
                AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("Domain")));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationPermissionDeclaration");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclarationOrganization");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclarationRole");

            migrationBuilder.DropTable(
                name: "RolePermissionDeclaration");

            migrationBuilder.DropTable(
                name: "UserPermissionDeclaration");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclaration");

            migrationBuilder.DropTable(
                name: "PermissionDefinition");

            migrationBuilder.DropTable(
                name: "RequestAuthorizationRule");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "TreeDomains");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Domains");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "UserLogins",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "TreeDomains",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "Domains",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppUserTokens",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppUsers",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppUserRoles",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppUserClaims",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppRoles",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreationTime",
                table: "AppRoleClaims",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValueSql: "sysDateTimeOffset()");
        }
    }
}
