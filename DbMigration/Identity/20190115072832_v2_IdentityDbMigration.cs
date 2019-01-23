using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbMigration.Identity
{
    public partial class v2_IdentityDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplexProperty2_ComplexProperty_ComplexEntityPropertyDomainId",
                table: "ComplexProperty2");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_AppUsers_CreationUserId",
                table: "Organization");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_AppUsers_LastModificationUserId",
                table: "Organization");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_Organization_ParentId",
                table: "Organization");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclarations_Organization_OrganizationId",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclarations_OrganizationPermissionDeclarations_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclarationOrganizations");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclarationRoles");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclarations");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPermissionDeclarations_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations");

            //migrationBuilder.DropUniqueConstraint(
            //    name: "AK_ComplexProperty_TempId",
            //    table: "ComplexProperty");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organization",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations");

            //migrationBuilder.DropColumn(
            //    name: "TempId",
            //    table: "ComplexProperty");

            migrationBuilder.RenameTable(
                name: "Organization",
                newName: "Organizations");

            migrationBuilder.RenameColumn(
                name: "MethodSignName",
                table: "RequestAuthorizationRules",
                newName: "IdentificationKey");

            migrationBuilder.RenameColumn(
                name: "FriendlyName",
                table: "RequestAuthorizationRules",
                newName: "HandlerMethodSignature");

            migrationBuilder.RenameColumn(
                name: "AdvanceAuthorizationRuleJson",
                table: "RequestAuthorizationRules",
                newName: "AuthorizationRuleConfigJson");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_ParentId",
                table: "Organizations",
                newName: "IX_Organizations_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_LastModificationUserId",
                table: "Organizations",
                newName: "IX_Organizations_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Organization_CreationUserId",
                table: "Organizations",
                newName: "IX_Organizations_CreationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organizations",
                table: "Organizations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplexProperty2_ComplexProperty_ComplexEntityPropertyDomainId",
                table: "ComplexProperty2",
                column: "ComplexEntityPropertyDomainId",
                principalTable: "ComplexProperty",
                principalColumn: "DomainId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclarations_Organizations_OrganizationId",
                table: "OrganizationPermissionDeclarations",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AppUsers_CreationUserId",
                table: "Organizations",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_AppUsers_LastModificationUserId",
                table: "Organizations",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Organizations_ParentId",
                table: "Organizations",
                column: "ParentId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplexProperty2_ComplexProperty_ComplexEntityPropertyDomainId",
                table: "ComplexProperty2");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclarations_Organizations_OrganizationId",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AppUsers_CreationUserId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_AppUsers_LastModificationUserId",
                table: "Organizations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Organizations_ParentId",
                table: "Organizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organizations",
                table: "Organizations");

            migrationBuilder.RenameTable(
                name: "Organizations",
                newName: "Organization");

            migrationBuilder.RenameColumn(
                name: "IdentificationKey",
                table: "RequestAuthorizationRules",
                newName: "MethodSignName");

            migrationBuilder.RenameColumn(
                name: "HandlerMethodSignature",
                table: "RequestAuthorizationRules",
                newName: "FriendlyName");

            migrationBuilder.RenameColumn(
                name: "AuthorizationRuleConfigJson",
                table: "RequestAuthorizationRules",
                newName: "AdvanceAuthorizationRuleJson");

            migrationBuilder.RenameIndex(
                name: "IX_Organizations_ParentId",
                table: "Organization",
                newName: "IX_Organization_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Organizations_LastModificationUserId",
                table: "Organization",
                newName: "IX_Organization_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Organizations_CreationUserId",
                table: "Organization",
                newName: "IX_Organization_CreationUserId");

            migrationBuilder.AddColumn<string>(
                name: "ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "TempId",
            //    table: "ComplexProperty",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddUniqueConstraint(
            //    name: "AK_ComplexProperty_TempId",
            //    table: "ComplexProperty",
            //    column: "TempId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organization",
                table: "Organization",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RequestHandlerPermissionDeclarations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    CreationUserId = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
                    PermissionValue = table.Column<short>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    RuleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHandlerPermissionDeclarations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarations_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarations_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarations_RequestAuthorizationRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "RequestAuthorizationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestHandlerPermissionDeclarationOrganizations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    CreationUserId = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrganizationId = table.Column<string>(nullable: true),
                    PermissionDeclarationId = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHandlerPermissionDeclarationOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationOrganizations_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationOrganizations_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationOrganizations_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationOrganizations_RequestHandlerPermissionDeclarations_PermissionDeclarationId",
                        column: x => x.PermissionDeclarationId,
                        principalTable: "RequestHandlerPermissionDeclarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestHandlerPermissionDeclarationRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    CreationUserId = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PermissionDeclarationId = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestHandlerPermissionDeclarationRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationRoles_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationRoles_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationRoles_RequestHandlerPermissionDeclarations_PermissionDeclarationId",
                        column: x => x.PermissionDeclarationId,
                        principalTable: "RequestHandlerPermissionDeclarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestHandlerPermissionDeclarationRoles_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclarations_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations",
                column: "ParentPermissionDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganizations_CreationUserId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganizations_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganizations_OrganizationId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganizations_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "PermissionDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationRoles_CreationUserId",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationRoles_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationRoles_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "PermissionDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarationRoles_RoleId",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarations_CreationUserId",
                table: "RequestHandlerPermissionDeclarations",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarations_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarations",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarations_PermissionDefinitionId",
                table: "RequestHandlerPermissionDeclarations",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHandlerPermissionDeclarations_RuleId",
                table: "RequestHandlerPermissionDeclarations",
                column: "RuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplexProperty2_ComplexProperty_ComplexEntityPropertyDomainId",
                table: "ComplexProperty2",
                column: "ComplexEntityPropertyDomainId",
                principalTable: "ComplexProperty",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_AppUsers_CreationUserId",
                table: "Organization",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_AppUsers_LastModificationUserId",
                table: "Organization",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_Organization_ParentId",
                table: "Organization",
                column: "ParentId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclarations_Organization_OrganizationId",
                table: "OrganizationPermissionDeclarations",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclarations_OrganizationPermissionDeclarations_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations",
                column: "ParentPermissionDeclarationId",
                principalTable: "OrganizationPermissionDeclarations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
