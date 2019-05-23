using System;
using System.Collections.Generic;
using CoreDX.Application.Domain.Model.Entity.Core;
using CoreDX.Application.Domain.Model.Entity.Security;
using CoreDX.Infrastructure.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreDX.Infrastructure.EntityFrameworkCore.EntityConfiguration
{
    public static class ApplicationDomainEntityConfiguration
    {
        public static void ConfigPermissionDefinition<TPermissionDefinition, TIdentityUser, TKey>(
            this EntityTypeBuilder<TPermissionDefinition> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityUser : class, IEntity<TKey>
            where TPermissionDefinition : PermissionDefinition<TKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TPermissionDefinition, TIdentityUser, TKey>();
            builder.ToTable("PermissionDefinitions");
        }

        public static void ConfigUserPermissionDeclaration<TUserPermissionDeclaration, TIdentityUser, TKey>(
            this EntityTypeBuilder<TUserPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityUser : class, IEntity<TKey>
            where TUserPermissionDeclaration : UserPermissionDeclaration<TKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TUserPermissionDeclaration, TIdentityUser, TKey>();
            builder.HasOne(up => up.User)
                .WithMany(u => (IEnumerable<TUserPermissionDeclaration>)u.PermissionDeclarations)
                .HasForeignKey(up => up.UserId);
            builder.ToTable("UserPermissionDeclarations");
        }

        public static void ConfigRolePermissionDeclaration<TRolePermissionDeclaration, TIdentityUser, TKey>(
            this EntityTypeBuilder<TRolePermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityUser : class, IEntity<TKey>
            where TRolePermissionDeclaration : RolePermissionDeclaration<TKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TRolePermissionDeclaration, TIdentityUser, TKey>();
            builder.HasOne(rp => rp.Role)
                .WithMany(r => (IEnumerable<TRolePermissionDeclaration>)r.PermissionDeclarations)
                .HasForeignKey(rp => rp.RoleId);
            builder.ToTable("RolePermissionDeclarations");
        }

        public static void ConfigOrganizationPermissionDeclaration<TOrganizationPermissionDeclaration, TIdentityUser, TKey>(
            this EntityTypeBuilder<TOrganizationPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityUser : class, IEntity<TKey>
            where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TOrganizationPermissionDeclaration, TIdentityUser, TKey>();
            builder.HasOne(op => op.Organization)
                .WithMany(o => (IEnumerable<TOrganizationPermissionDeclaration>)o.PermissionDeclarations)
                .HasForeignKey(op => op.OrganizationId);
            builder.ToTable("OrganizationPermissionDeclarations");
        }

        public static void ConfigOrganizationPermissionDeclaration1<TOrganizationPermissionDeclaration, TIdentityUser, TKey>(
            this EntityTypeBuilder<TOrganizationPermissionDeclaration> builder)
            where TKey : struct, IEquatable<TKey>
            where TIdentityUser : class, IEntity<TKey>
            where TOrganizationPermissionDeclaration : OrganizationPermissionDeclaration<TKey>
        {
            builder.ConfigForDomainEntityBase<TKey, TOrganizationPermissionDeclaration, TIdentityUser, TKey>();
            builder.HasOne(op => op.Organization)
                .WithMany(o => (IEnumerable<TOrganizationPermissionDeclaration>)o.PermissionDeclarations)
                .HasForeignKey(op => op.OrganizationId);
            builder.ToTable("OrganizationPermissionDeclarations");
        }
    }
}
