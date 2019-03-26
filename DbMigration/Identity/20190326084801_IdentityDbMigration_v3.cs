using Microsoft.EntityFrameworkCore.Migrations;

namespace DbMigration.Identity
{
    public partial class IdentityDbMigration_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserOrganizations_AppUsers_CreationUserId",
                table: "ApplicationUserOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserOrganizations_AppUsers_LastModificationUserId",
                table: "ApplicationUserOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserOrganizations_Organizations_OrganizationId",
                table: "ApplicationUserOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserOrganizations_AppUsers_UserId",
                table: "ApplicationUserOrganizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserOrganizations",
                table: "ApplicationUserOrganizations");

            migrationBuilder.RenameTable(
                name: "ApplicationUserOrganizations",
                newName: "AppUserOrganizations");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserOrganizations_UserId",
                table: "AppUserOrganizations",
                newName: "IX_AppUserOrganizations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserOrganizations_OrganizationId",
                table: "AppUserOrganizations",
                newName: "IX_AppUserOrganizations_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserOrganizations_LastModificationUserId",
                table: "AppUserOrganizations",
                newName: "IX_AppUserOrganizations_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserOrganizations_CreationUserId",
                table: "AppUserOrganizations",
                newName: "IX_AppUserOrganizations_CreationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUserOrganizations",
                table: "AppUserOrganizations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserOrganizations_AppUsers_CreationUserId",
                table: "AppUserOrganizations",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserOrganizations_AppUsers_LastModificationUserId",
                table: "AppUserOrganizations",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserOrganizations_Organizations_OrganizationId",
                table: "AppUserOrganizations",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserOrganizations_AppUsers_UserId",
                table: "AppUserOrganizations",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserOrganizations_AppUsers_CreationUserId",
                table: "AppUserOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserOrganizations_AppUsers_LastModificationUserId",
                table: "AppUserOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserOrganizations_Organizations_OrganizationId",
                table: "AppUserOrganizations");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserOrganizations_AppUsers_UserId",
                table: "AppUserOrganizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUserOrganizations",
                table: "AppUserOrganizations");

            migrationBuilder.RenameTable(
                name: "AppUserOrganizations",
                newName: "ApplicationUserOrganizations");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserOrganizations_UserId",
                table: "ApplicationUserOrganizations",
                newName: "IX_ApplicationUserOrganizations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserOrganizations_OrganizationId",
                table: "ApplicationUserOrganizations",
                newName: "IX_ApplicationUserOrganizations_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserOrganizations_LastModificationUserId",
                table: "ApplicationUserOrganizations",
                newName: "IX_ApplicationUserOrganizations_LastModificationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserOrganizations_CreationUserId",
                table: "ApplicationUserOrganizations",
                newName: "IX_ApplicationUserOrganizations_CreationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserOrganizations",
                table: "ApplicationUserOrganizations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserOrganizations_AppUsers_CreationUserId",
                table: "ApplicationUserOrganizations",
                column: "CreationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserOrganizations_AppUsers_LastModificationUserId",
                table: "ApplicationUserOrganizations",
                column: "LastModificationUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserOrganizations_Organizations_OrganizationId",
                table: "ApplicationUserOrganizations",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserOrganizations_AppUsers_UserId",
                table: "ApplicationUserOrganizations",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
