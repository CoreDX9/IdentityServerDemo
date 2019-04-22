using System;
using System.Linq;
using Domain.EntityFrameworkCore.Extensions;
using EntityFrameworkCore.Extensions.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbMigration.Application
{
    public partial class InitialApplicationDbMigration : Migration
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
                name: "AuthorizationRules",
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
                    AuthorizationRuleConfigJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizationRules_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthorizationRules_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "Menus",
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
                    Icon_Type = table.Column<string>(nullable: true),
                    Icon_Value = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Order = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Menus_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
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
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organizations_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organizations_Organizations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Organizations",
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
                    HandlerMethodSignature = table.Column<string>(nullable: true),
                    TypeFullName = table.Column<string>(nullable: true),
                    IdentificationKey = table.Column<string>(nullable: true),
                    AuthorizationRuleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAuthorizationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAuthorizationRules_AuthorizationRules_AuthorizationRuleId",
                        column: x => x.AuthorizationRuleId,
                        principalTable: "AuthorizationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "ComplexProperty",
                columns: table => new
                {
                    DomainId = table.Column<string>(nullable: false),
                    C1 = table.Column<string>(nullable: true),
                    C2 = table.Column<string>(nullable: true)
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
                name: "MenuItems",
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
                    Icon_Type = table.Column<string>(nullable: true),
                    Icon_Value = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    Order = table.Column<short>(nullable: false),
                    MenuId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuItems_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuItems_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserOrganizations",
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
                    table.PrimaryKey("PK_AppUserOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserOrganizations_AppUsers_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserOrganizations_AppUsers_LastModificationUserId",
                        column: x => x.LastModificationUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserOrganizations_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
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
                    OrganizationId = table.Column<string>(nullable: true)
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
                        name: "FK_OrganizationPermissionDeclarations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
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
                name: "ComplexProperty2",
                columns: table => new
                {
                    ComplexEntityPropertyDomainId = table.Column<string>(nullable: false),
                    C3 = table.Column<string>(nullable: true),
                    C4 = table.Column<string>(nullable: true)
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
                name: "IX_AppUserOrganizations_CreationUserId",
                table: "AppUserOrganizations",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserOrganizations_LastModificationUserId",
                table: "AppUserOrganizations",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserOrganizations_OrganizationId",
                table: "AppUserOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserOrganizations_UserId",
                table: "AppUserOrganizations",
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
                name: "IX_AuthorizationRules_CreationUserId",
                table: "AuthorizationRules",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationRules_LastModificationUserId",
                table: "AuthorizationRules",
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
                name: "IX_MenuItems_CreationUserId",
                table: "MenuItems",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_LastModificationUserId",
                table: "MenuItems",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_MenuId",
                table: "MenuItems",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_CreationUserId",
                table: "Menus",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_LastModificationUserId",
                table: "Menus",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                table: "Menus",
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
                name: "IX_OrganizationPermissionDeclarations_PermissionDefinitionId",
                table: "OrganizationPermissionDeclarations",
                column: "PermissionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CreationUserId",
                table: "Organizations",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_LastModificationUserId",
                table: "Organizations",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ParentId",
                table: "Organizations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDefinitions_CreationUserId",
                table: "PermissionDefinitions",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDefinitions_LastModificationUserId",
                table: "PermissionDefinitions",
                column: "LastModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuthorizationRules_AuthorizationRuleId",
                table: "RequestAuthorizationRules",
                column: "AuthorizationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuthorizationRules_CreationUserId",
                table: "RequestAuthorizationRules",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAuthorizationRules_LastModificationUserId",
                table: "RequestAuthorizationRules",
                column: "LastModificationUserId");

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
                .CreateIdentityTreeEntityView(this, AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains("Domain")))
                //从模型注解应用表和列说明
                .ApplyDatabaseDescription(this);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //删除树形实体视图，这个没办法自动扫描
            migrationBuilder.DropTreeEntityView("AppRoles")
                .DropTreeEntityView("TreeDomains")
                .DropTreeEntityView("Organizations")
                .DropTreeEntityView("Menus");

            migrationBuilder.DropTable(
                name: "AppRoleClaims");

            migrationBuilder.DropTable(
                name: "AppUserClaims");

            migrationBuilder.DropTable(
                name: "AppUserOrganizations");

            migrationBuilder.DropTable(
                name: "AppUserRoles");

            migrationBuilder.DropTable(
                name: "AppUserTokens");

            migrationBuilder.DropTable(
                name: "ComplexProperty2");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "OrganizationPermissionDeclarations");

            migrationBuilder.DropTable(
                name: "RequestAuthorizationRules");

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
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "AuthorizationRules");

            migrationBuilder.DropTable(
                name: "AppRoles");

            migrationBuilder.DropTable(
                name: "PermissionDefinitions");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "AppUsers");
        }
    }
}
