using Microsoft.EntityFrameworkCore.Migrations;

namespace DbMigration.Identity
{
    public partial class AddPermissionIdentityDbMigration_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclaration_AppUsers_CreationUserId",
                table: "OrganizationPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclaration_AppUsers_LastModificationUserId",
                table: "OrganizationPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclaration_Organization_OrganizationId",
                table: "OrganizationPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclaration_OrganizationPermissionDeclaration_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                table: "OrganizationPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionDefinition_AppUsers_CreationUserId",
                table: "PermissionDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionDefinition_AppUsers_LastModificationUserId",
                table: "PermissionDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestAuthorizationRule_AppUsers_CreationUserId",
                table: "RequestAuthorizationRule");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestAuthorizationRule_AppUsers_LastModificationUserId",
                table: "RequestAuthorizationRule");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclaration_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclaration_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                table: "RequestHandlerPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclaration_RequestAuthorizationRule_RuleId",
                table: "RequestHandlerPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganization_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarationOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganization_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganization_Organization_OrganizationId",
                table: "RequestHandlerPermissionDeclarationOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganization_RequestHandlerPermissionDeclaration_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationOrganization");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRole_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarationRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRole_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRole_RequestHandlerPermissionDeclaration_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRole_AppRoles_RoleId",
                table: "RequestHandlerPermissionDeclarationRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionDeclaration_AppUsers_CreationUserId",
                table: "RolePermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionDeclaration_AppUsers_LastModificationUserId",
                table: "RolePermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                table: "RolePermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionDeclaration_AppRoles_RoleId",
                table: "RolePermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDeclaration_AppUsers_CreationUserId",
                table: "UserPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDeclaration_AppUsers_LastModificationUserId",
                table: "UserPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                table: "UserPermissionDeclaration");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDeclaration_AppUsers_UserId",
                table: "UserPermissionDeclaration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissionDeclaration",
                table: "UserPermissionDeclaration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissionDeclaration",
                table: "RolePermissionDeclaration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarationRole",
                table: "RequestHandlerPermissionDeclarationRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarationOrganization",
                table: "RequestHandlerPermissionDeclarationOrganization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclaration",
                table: "RequestHandlerPermissionDeclaration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestAuthorizationRule",
                table: "RequestAuthorizationRule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionDefinition",
                table: "PermissionDefinition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationPermissionDeclaration",
                table: "OrganizationPermissionDeclaration");

            migrationBuilder.RenameTable(
                name: "UserPermissionDeclaration",
                newName: "UserPermissionDeclarations");

            migrationBuilder.RenameTable(
                name: "RolePermissionDeclaration",
                newName: "RolePermissionDeclarations");

            migrationBuilder.RenameTable(
                name: "RequestHandlerPermissionDeclarationRole",
                newName: "RequestHandlerPermissionDeclarationRoles");

            migrationBuilder.RenameTable(
                name: "RequestHandlerPermissionDeclarationOrganization",
                newName: "RequestHandlerPermissionDeclarationOrganizations");

            migrationBuilder.RenameTable(
                name: "RequestHandlerPermissionDeclaration",
                newName: "RequestHandlerPermissionDeclarations");

            migrationBuilder.RenameTable(
                name: "RequestAuthorizationRule",
                newName: "RequestAuthorizationRules");

            migrationBuilder.RenameTable(
                name: "PermissionDefinition",
                newName: "PermissionDefinitions");

            migrationBuilder.RenameTable(
                name: "OrganizationPermissionDeclaration",
                newName: "OrganizationPermissionDeclarations");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionDeclaration_UserId",
                table: "UserPermissionDeclarations",
                newName: "IX_UserPermissionDeclarations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionDeclaration_PermissionDefinitionId",
                table: "UserPermissionDeclarations",
                newName: "IX_UserPermissionDeclarations_PermissionDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionDeclaration_LastModificationUserId",
                table: "UserPermissionDeclarations",
                newName: "IX_UserPermissionDeclarations_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionDeclaration_CreationUserId",
                table: "UserPermissionDeclarations",
                newName: "IX_UserPermissionDeclarations_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissionDeclaration_RoleId",
                table: "RolePermissionDeclarations",
                newName: "IX_RolePermissionDeclarations_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissionDeclaration_PermissionDefinitionId",
                table: "RolePermissionDeclarations",
                newName: "IX_RolePermissionDeclarations_PermissionDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissionDeclaration_LastModificationUserId",
                table: "RolePermissionDeclarations",
                newName: "IX_RolePermissionDeclarations_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissionDeclaration_CreationUserId",
                table: "RolePermissionDeclarations",
                newName: "IX_RolePermissionDeclarations_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationRole_RoleId",
                table: "RequestHandlerPermissionDeclarationRoles",
                newName: "IX_RequestHandlerPermissionDeclarationRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationRole_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationRoles",
                newName: "IX_RequestHandlerPermissionDeclarationRoles_PermissionDeclarationId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationRole_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationRoles",
                newName: "IX_RequestHandlerPermissionDeclarationRoles_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationRole_CreationUserId",
                table: "RequestHandlerPermissionDeclarationRoles",
                newName: "IX_RequestHandlerPermissionDeclarationRoles_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganization_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                newName: "IX_RequestHandlerPermissionDeclarationOrganizations_PermissionDeclarationId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganization_OrganizationId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                newName: "IX_RequestHandlerPermissionDeclarationOrganizations_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganization_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                newName: "IX_RequestHandlerPermissionDeclarationOrganizations_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganization_CreationUserId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                newName: "IX_RequestHandlerPermissionDeclarationOrganizations_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclaration_RuleId",
                table: "RequestHandlerPermissionDeclarations",
                newName: "IX_RequestHandlerPermissionDeclarations_RuleId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclaration_PermissionDefinitionId",
                table: "RequestHandlerPermissionDeclarations",
                newName: "IX_RequestHandlerPermissionDeclarations_PermissionDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclaration_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarations",
                newName: "IX_RequestHandlerPermissionDeclarations_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclaration_CreationUserId",
                table: "RequestHandlerPermissionDeclarations",
                newName: "IX_RequestHandlerPermissionDeclarations_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestAuthorizationRule_LastModificationUserId",
                table: "RequestAuthorizationRules",
                newName: "IX_RequestAuthorizationRules_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestAuthorizationRule_CreationUserId",
                table: "RequestAuthorizationRules",
                newName: "IX_RequestAuthorizationRules_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionDefinition_LastModificationUserId",
                table: "PermissionDefinitions",
                newName: "IX_PermissionDefinitions_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionDefinition_CreationUserId",
                table: "PermissionDefinitions",
                newName: "IX_PermissionDefinitions_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclaration_PermissionDefinitionId",
                table: "OrganizationPermissionDeclarations",
                newName: "IX_OrganizationPermissionDeclarations_PermissionDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclaration_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations",
                newName: "IX_OrganizationPermissionDeclarations_ParentPermissionDeclarationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclaration_OrganizationId",
                table: "OrganizationPermissionDeclarations",
                newName: "IX_OrganizationPermissionDeclarations_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclaration_LastModificationUserId",
                table: "OrganizationPermissionDeclarations",
                newName: "IX_OrganizationPermissionDeclarations_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclaration_CreationUserId",
                table: "OrganizationPermissionDeclarations",
                newName: "IX_OrganizationPermissionDeclarations_CreationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissionDeclarations",
                table: "UserPermissionDeclarations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissionDeclarations",
                table: "RolePermissionDeclarations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarationRoles",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarationOrganizations",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarations",
                table: "RequestHandlerPermissionDeclarations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestAuthorizationRules",
                table: "RequestAuthorizationRules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionDefinitions",
                table: "PermissionDefinitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationPermissionDeclarations",
                table: "OrganizationPermissionDeclarations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclarations_AppUsers_CreationUserId",
                table: "OrganizationPermissionDeclarations",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclarations_AppUsers_LastModificationUserId",
                table: "OrganizationPermissionDeclarations",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
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

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                table: "OrganizationPermissionDeclarations",
                column: "PermissionDefinitionId",
                principalTable: "PermissionDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionDefinitions_AppUsers_CreationUserId",
                table: "PermissionDefinitions",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionDefinitions_AppUsers_LastModificationUserId",
                table: "PermissionDefinitions",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestAuthorizationRules_AppUsers_CreationUserId",
                table: "RequestAuthorizationRules",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestAuthorizationRules_AppUsers_LastModificationUserId",
                table: "RequestAuthorizationRules",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganizations_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganizations_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganizations_Organization_OrganizationId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganizations_RequestHandlerPermissionDeclarations_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationOrganizations",
                column: "PermissionDeclarationId",
                principalTable: "RequestHandlerPermissionDeclarations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRoles_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRoles_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRoles_RequestHandlerPermissionDeclarations_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "PermissionDeclarationId",
                principalTable: "RequestHandlerPermissionDeclarations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRoles_AppRoles_RoleId",
                table: "RequestHandlerPermissionDeclarationRoles",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarations_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarations",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarations_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarations",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                table: "RequestHandlerPermissionDeclarations",
                column: "PermissionDefinitionId",
                principalTable: "PermissionDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarations_RequestAuthorizationRules_RuleId",
                table: "RequestHandlerPermissionDeclarations",
                column: "RuleId",
                principalTable: "RequestAuthorizationRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionDeclarations_AppUsers_CreationUserId",
                table: "RolePermissionDeclarations",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionDeclarations_AppUsers_LastModificationUserId",
                table: "RolePermissionDeclarations",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                table: "RolePermissionDeclarations",
                column: "PermissionDefinitionId",
                principalTable: "PermissionDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionDeclarations_AppRoles_RoleId",
                table: "RolePermissionDeclarations",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDeclarations_AppUsers_CreationUserId",
                table: "UserPermissionDeclarations",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDeclarations_AppUsers_LastModificationUserId",
                table: "UserPermissionDeclarations",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                table: "UserPermissionDeclarations",
                column: "PermissionDefinitionId",
                principalTable: "PermissionDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDeclarations_AppUsers_UserId",
                table: "UserPermissionDeclarations",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclarations_AppUsers_CreationUserId",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclarations_AppUsers_LastModificationUserId",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclarations_Organization_OrganizationId",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclarations_OrganizationPermissionDeclarations_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionDefinitions_AppUsers_CreationUserId",
                table: "PermissionDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionDefinitions_AppUsers_LastModificationUserId",
                table: "PermissionDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestAuthorizationRules_AppUsers_CreationUserId",
                table: "RequestAuthorizationRules");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestAuthorizationRules_AppUsers_LastModificationUserId",
                table: "RequestAuthorizationRules");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganizations_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarationOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganizations_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganizations_Organization_OrganizationId",
                table: "RequestHandlerPermissionDeclarationOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganizations_RequestHandlerPermissionDeclarations_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRoles_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarationRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRoles_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRoles_RequestHandlerPermissionDeclarations_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRoles_AppRoles_RoleId",
                table: "RequestHandlerPermissionDeclarationRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarations_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarations_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                table: "RequestHandlerPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestHandlerPermissionDeclarations_RequestAuthorizationRules_RuleId",
                table: "RequestHandlerPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionDeclarations_AppUsers_CreationUserId",
                table: "RolePermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionDeclarations_AppUsers_LastModificationUserId",
                table: "RolePermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                table: "RolePermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissionDeclarations_AppRoles_RoleId",
                table: "RolePermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDeclarations_AppUsers_CreationUserId",
                table: "UserPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDeclarations_AppUsers_LastModificationUserId",
                table: "UserPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                table: "UserPermissionDeclarations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDeclarations_AppUsers_UserId",
                table: "UserPermissionDeclarations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissionDeclarations",
                table: "UserPermissionDeclarations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissionDeclarations",
                table: "RolePermissionDeclarations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarations",
                table: "RequestHandlerPermissionDeclarations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarationRoles",
                table: "RequestHandlerPermissionDeclarationRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarationOrganizations",
                table: "RequestHandlerPermissionDeclarationOrganizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestAuthorizationRules",
                table: "RequestAuthorizationRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionDefinitions",
                table: "PermissionDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationPermissionDeclarations",
                table: "OrganizationPermissionDeclarations");

            migrationBuilder.RenameTable(
                name: "UserPermissionDeclarations",
                newName: "UserPermissionDeclaration");

            migrationBuilder.RenameTable(
                name: "RolePermissionDeclarations",
                newName: "RolePermissionDeclaration");

            migrationBuilder.RenameTable(
                name: "RequestHandlerPermissionDeclarations",
                newName: "RequestHandlerPermissionDeclaration");

            migrationBuilder.RenameTable(
                name: "RequestHandlerPermissionDeclarationRoles",
                newName: "RequestHandlerPermissionDeclarationRole");

            migrationBuilder.RenameTable(
                name: "RequestHandlerPermissionDeclarationOrganizations",
                newName: "RequestHandlerPermissionDeclarationOrganization");

            migrationBuilder.RenameTable(
                name: "RequestAuthorizationRules",
                newName: "RequestAuthorizationRule");

            migrationBuilder.RenameTable(
                name: "PermissionDefinitions",
                newName: "PermissionDefinition");

            migrationBuilder.RenameTable(
                name: "OrganizationPermissionDeclarations",
                newName: "OrganizationPermissionDeclaration");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionDeclarations_UserId",
                table: "UserPermissionDeclaration",
                newName: "IX_UserPermissionDeclaration_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionDeclarations_PermissionDefinitionId",
                table: "UserPermissionDeclaration",
                newName: "IX_UserPermissionDeclaration_PermissionDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionDeclarations_LastModificationUserId",
                table: "UserPermissionDeclaration",
                newName: "IX_UserPermissionDeclaration_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPermissionDeclarations_CreationUserId",
                table: "UserPermissionDeclaration",
                newName: "IX_UserPermissionDeclaration_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissionDeclarations_RoleId",
                table: "RolePermissionDeclaration",
                newName: "IX_RolePermissionDeclaration_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissionDeclarations_PermissionDefinitionId",
                table: "RolePermissionDeclaration",
                newName: "IX_RolePermissionDeclaration_PermissionDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissionDeclarations_LastModificationUserId",
                table: "RolePermissionDeclaration",
                newName: "IX_RolePermissionDeclaration_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissionDeclarations_CreationUserId",
                table: "RolePermissionDeclaration",
                newName: "IX_RolePermissionDeclaration_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarations_RuleId",
                table: "RequestHandlerPermissionDeclaration",
                newName: "IX_RequestHandlerPermissionDeclaration_RuleId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarations_PermissionDefinitionId",
                table: "RequestHandlerPermissionDeclaration",
                newName: "IX_RequestHandlerPermissionDeclaration_PermissionDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarations_LastModificationUserId",
                table: "RequestHandlerPermissionDeclaration",
                newName: "IX_RequestHandlerPermissionDeclaration_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarations_CreationUserId",
                table: "RequestHandlerPermissionDeclaration",
                newName: "IX_RequestHandlerPermissionDeclaration_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationRoles_RoleId",
                table: "RequestHandlerPermissionDeclarationRole",
                newName: "IX_RequestHandlerPermissionDeclarationRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationRoles_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationRole",
                newName: "IX_RequestHandlerPermissionDeclarationRole_PermissionDeclarationId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationRoles_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationRole",
                newName: "IX_RequestHandlerPermissionDeclarationRole_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationRoles_CreationUserId",
                table: "RequestHandlerPermissionDeclarationRole",
                newName: "IX_RequestHandlerPermissionDeclarationRole_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganizations_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                newName: "IX_RequestHandlerPermissionDeclarationOrganization_PermissionDeclarationId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganizations_OrganizationId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                newName: "IX_RequestHandlerPermissionDeclarationOrganization_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganizations_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                newName: "IX_RequestHandlerPermissionDeclarationOrganization_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestHandlerPermissionDeclarationOrganizations_CreationUserId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                newName: "IX_RequestHandlerPermissionDeclarationOrganization_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestAuthorizationRules_LastModificationUserId",
                table: "RequestAuthorizationRule",
                newName: "IX_RequestAuthorizationRule_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestAuthorizationRules_CreationUserId",
                table: "RequestAuthorizationRule",
                newName: "IX_RequestAuthorizationRule_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionDefinitions_LastModificationUserId",
                table: "PermissionDefinition",
                newName: "IX_PermissionDefinition_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionDefinitions_CreationUserId",
                table: "PermissionDefinition",
                newName: "IX_PermissionDefinition_CreationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclarations_PermissionDefinitionId",
                table: "OrganizationPermissionDeclaration",
                newName: "IX_OrganizationPermissionDeclaration_PermissionDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclarations_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclaration",
                newName: "IX_OrganizationPermissionDeclaration_ParentPermissionDeclarationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclarations_OrganizationId",
                table: "OrganizationPermissionDeclaration",
                newName: "IX_OrganizationPermissionDeclaration_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclarations_LastModificationUserId",
                table: "OrganizationPermissionDeclaration",
                newName: "IX_OrganizationPermissionDeclaration_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationPermissionDeclarations_CreationUserId",
                table: "OrganizationPermissionDeclaration",
                newName: "IX_OrganizationPermissionDeclaration_CreationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissionDeclaration",
                table: "UserPermissionDeclaration",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissionDeclaration",
                table: "RolePermissionDeclaration",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclaration",
                table: "RequestHandlerPermissionDeclaration",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarationRole",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestHandlerPermissionDeclarationOrganization",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestAuthorizationRule",
                table: "RequestAuthorizationRule",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionDefinition",
                table: "PermissionDefinition",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationPermissionDeclaration",
                table: "OrganizationPermissionDeclaration",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclaration_AppUsers_CreationUserId",
                table: "OrganizationPermissionDeclaration",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclaration_AppUsers_LastModificationUserId",
                table: "OrganizationPermissionDeclaration",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclaration_Organization_OrganizationId",
                table: "OrganizationPermissionDeclaration",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclaration_OrganizationPermissionDeclaration_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclaration",
                column: "ParentPermissionDeclarationId",
                principalTable: "OrganizationPermissionDeclaration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                table: "OrganizationPermissionDeclaration",
                column: "PermissionDefinitionId",
                principalTable: "PermissionDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionDefinition_AppUsers_CreationUserId",
                table: "PermissionDefinition",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionDefinition_AppUsers_LastModificationUserId",
                table: "PermissionDefinition",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestAuthorizationRule_AppUsers_CreationUserId",
                table: "RequestAuthorizationRule",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestAuthorizationRule_AppUsers_LastModificationUserId",
                table: "RequestAuthorizationRule",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclaration_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclaration",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclaration_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclaration",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                table: "RequestHandlerPermissionDeclaration",
                column: "PermissionDefinitionId",
                principalTable: "PermissionDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclaration_RequestAuthorizationRule_RuleId",
                table: "RequestHandlerPermissionDeclaration",
                column: "RuleId",
                principalTable: "RequestAuthorizationRule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganization_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganization_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganization_Organization_OrganizationId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationOrganization_RequestHandlerPermissionDeclaration_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationOrganization",
                column: "PermissionDeclarationId",
                principalTable: "RequestHandlerPermissionDeclaration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRole_AppUsers_CreationUserId",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRole_AppUsers_LastModificationUserId",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRole_RequestHandlerPermissionDeclaration_PermissionDeclarationId",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "PermissionDeclarationId",
                principalTable: "RequestHandlerPermissionDeclaration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHandlerPermissionDeclarationRole_AppRoles_RoleId",
                table: "RequestHandlerPermissionDeclarationRole",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionDeclaration_AppUsers_CreationUserId",
                table: "RolePermissionDeclaration",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionDeclaration_AppUsers_LastModificationUserId",
                table: "RolePermissionDeclaration",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                table: "RolePermissionDeclaration",
                column: "PermissionDefinitionId",
                principalTable: "PermissionDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissionDeclaration_AppRoles_RoleId",
                table: "RolePermissionDeclaration",
                column: "RoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDeclaration_AppUsers_CreationUserId",
                table: "UserPermissionDeclaration",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDeclaration_AppUsers_LastModificationUserId",
                table: "UserPermissionDeclaration",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDeclaration_PermissionDefinition_PermissionDefinitionId",
                table: "UserPermissionDeclaration",
                column: "PermissionDefinitionId",
                principalTable: "PermissionDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDeclaration_AppUsers_UserId",
                table: "UserPermissionDeclaration",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
