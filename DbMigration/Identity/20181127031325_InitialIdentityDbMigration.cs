using System;
using System.Linq;
using Domain.EntityFrameworkCore.Extensions;
using EntityFrameworkCore.Extensions.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbMigration.Identity
{
    public partial class InitialIdentityDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    Sex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUsers_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUsers_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRoles_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppRoles_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppRoles_AppRoles_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserClaims_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserClaims_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserClaims_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AppUserTokens_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserTokens_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserTokens_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Domains",
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
                    SC = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Domains_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Domains_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
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
                name: "PermissionDefinitions",
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
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ValueType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionDefinitions_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermissionDefinitions_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestAuthorizationRules",
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
                    MethodSignName = table.Column<string>(nullable: true),
                    TypeFullName = table.Column<string>(nullable: true),
                    FriendlyName = table.Column<string>(nullable: true),
                    AdvanceAuthorizationRuleJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAuthorizationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAuthorizationRules_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestAuthorizationRules_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreeDomains",
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
                    ParentId = table.Column<string>(nullable: true),
                    SampleColumn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeDomains_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeDomains_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeDomains_TreeDomains_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TreeDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogins_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogins_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<long>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRoleClaims_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppRoleClaims_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppRoleClaims_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    OrderNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false, defaultValueSql: "'True'"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "'False'"),
                    CreationTime = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "sysDateTimeOffset()"),
                    LastModificationTime = table.Column<DateTimeOffset>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    LastModificationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AppUserRoles_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserRoles_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserRoles_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserRoles_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComplexProperty",
                columns: table => new
                {
                    C1 = table.Column<string>(nullable: true),
                    C2 = table.Column<string>(nullable: true),
                    DomainId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplexProperty", x => x.DomainId);
                    table.ForeignKey(
                        name: "FK_ComplexProperty_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPermissionDeclarations",
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
                    PermissionValue = table.Column<short>(nullable: false),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<string>(nullable: true),
                    ParentPermissionDeclarationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPermissionDeclarations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclarations_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclarations_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclarations_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclarations_OrganizationPermissionDeclarations_ParentPermissionDeclarationId",
                        column: x => x.ParentPermissionDeclarationId,
                        principalTable: "OrganizationPermissionDeclarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissionDeclarations",
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
                    PermissionValue = table.Column<short>(nullable: false),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissionDeclarations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclarations_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclarations_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissionDeclarations_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionDeclarations",
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
                    PermissionValue = table.Column<short>(nullable: false),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionDeclarations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclarations_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclarations_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclarations_PermissionDefinitions_PermissionDefinitionId",
                        column: x => x.PermissionDefinitionId,
                        principalTable: "PermissionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPermissionDeclarations_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestHandlerPermissionDeclarations",
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
                    PermissionValue = table.Column<short>(nullable: false),
                    PermissionDefinitionId = table.Column<string>(nullable: true),
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
                name: "ComplexProperty2",
                columns: table => new
                {
                    C3 = table.Column<string>(nullable: true),
                    C4 = table.Column<string>(nullable: true),
                    ComplexEntityPropertyDomainId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplexProperty2", x => x.ComplexEntityPropertyDomainId);
                    table.ForeignKey(
                        name: "FK_ComplexProperty2_ComplexProperty_ComplexEntityPropertyDomainId",
                        column: x => x.ComplexEntityPropertyDomainId,
                        principalTable: "ComplexProperty",
                        principalColumn: "DomainId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestHandlerPermissionDeclarationOrganizations",
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
                    OrganizationId = table.Column<string>(nullable: true),
                    PermissionDeclarationId = table.Column<string>(nullable: true)
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
                    RoleId = table.Column<string>(nullable: true),
                    PermissionDeclarationId = table.Column<string>(nullable: true)
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
                name: "IX_AppRoleClaims_CreationUserId",
                table: "AppRoleClaims",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoleClaims_LastModificationUserId",
                table: "AppRoleClaims",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoleClaims_RoleId",
                table: "AppRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoles_CreationUserId",
                table: "AppRoles",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoles_LastModificationUserId",
                table: "AppRoles",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AppRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoles_ParentId",
                table: "AppRoles",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserClaims_CreationUserId",
                table: "AppUserClaims",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserClaims_LastModificationUserId",
                table: "AppUserClaims",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserClaims_UserId",
                table: "AppUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRoles_CreationUserId",
                table: "AppUserRoles",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRoles_LastModificationUserId",
                table: "AppUserRoles",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRoles_RoleId",
                table: "AppUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_CreationUserId",
                table: "AppUsers",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_LastModificationUserId",
                table: "AppUsers",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AppUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AppUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserTokens_CreationUserId",
                table: "AppUserTokens",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserTokens_LastModificationUserId",
                table: "AppUserTokens",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_CreationUserId",
                table: "Domains",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_LastModificationUserId",
                table: "Domains",
                column: "LastModificationUserId");

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
                name: "IX_OrganizationPermissionDeclarations_CreationUserId",
                table: "OrganizationPermissionDeclarations",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclarations_LastModificationUserId",
                table: "OrganizationPermissionDeclarations",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclarations_OrganizationId",
                table: "OrganizationPermissionDeclarations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclarations_ParentPermissionDeclarationId",
                table: "OrganizationPermissionDeclarations",
                column: "ParentPermissionDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPermissionDeclarations_PermissionDefinitionId",
                table: "OrganizationPermissionDeclarations",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDefinitions_CreationUserId",
                table: "PermissionDefinitions",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDefinitions_LastModificationUserId",
                table: "PermissionDefinitions",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuthorizationRules_CreationUserId",
                table: "RequestAuthorizationRules",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuthorizationRules_LastModificationUserId",
                table: "RequestAuthorizationRules",
                column: "LastModificationUserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclarations_CreationUserId",
                table: "RolePermissionDeclarations",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclarations_LastModificationUserId",
                table: "RolePermissionDeclarations",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclarations_PermissionDefinitionId",
                table: "RolePermissionDeclarations",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissionDeclarations_RoleId",
                table: "RolePermissionDeclarations",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeDomains_CreationUserId",
                table: "TreeDomains",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeDomains_LastModificationUserId",
                table: "TreeDomains",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeDomains_ParentId",
                table: "TreeDomains",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_CreationUserId",
                table: "UserLogins",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_LastModificationUserId",
                table: "UserLogins",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclarations_CreationUserId",
                table: "UserPermissionDeclarations",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclarations_LastModificationUserId",
                table: "UserPermissionDeclarations",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclarations_PermissionDefinitionId",
                table: "UserPermissionDeclarations",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionDeclarations_UserId",
                table: "UserPermissionDeclarations",
                column: "UserId");

            //自动扫描迁移模型并创建树形实体视图
            migrationBuilder.CreateTreeEntityView(this,
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("Domain")))
                .CreateIdentityTreeEntityView(this,
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("Domain")))
                //从模型注解应用表和列说明
                .ApplyDatabaseDescription(this);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //删除树形实体视图，这个没办法自动扫描
            migrationBuilder.DropTreeEntityView("AppRoles")
                .DropTreeEntityView("TreeDomains");

            migrationBuilder.DropTable(
                name: "AppRoleClaims");

            migrationBuilder.DropTable(
                name: "AppUserClaims");

            migrationBuilder.DropTable(
                name: "AppUserRoles");

            migrationBuilder.DropTable(
                name: "AppUserTokens");

            migrationBuilder.DropTable(
                name: "ComplexProperty2");

            migrationBuilder.DropTable(
                name: "OrganizationPermissionDeclarations");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclarationOrganizations");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclarationRoles");

            migrationBuilder.DropTable(
                name: "RolePermissionDeclarations");

            migrationBuilder.DropTable(
                name: "TreeDomains");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserPermissionDeclarations");

            migrationBuilder.DropTable(
                name: "ComplexProperty");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "RequestHandlerPermissionDeclarations");

            migrationBuilder.DropTable(
                name: "AppRoles");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "PermissionDefinitions");

            migrationBuilder.DropTable(
                name: "RequestAuthorizationRules");

            migrationBuilder.DropTable(
                name: "AppUsers");
        }
    }
}
