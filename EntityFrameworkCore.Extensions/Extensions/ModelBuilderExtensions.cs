using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntityFrameworkCore.Extensions.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Util.TypeExtensions;

namespace EntityFrameworkCore.Extensions.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// [UniqueKey(groupId: "1", order: 0)]
        /// public string ColumnName { get; set; }
        ///
        /// [UniqueKey(groupId: "1", order: 1)]
        /// public string ColumnName2 { get; set; }
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static ModelBuilder ConfigUniqueKey(this ModelBuilder modelBuilder)
        {
            // Iterate through all EF Entity types
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                #region Convert UniqueKeyAttribute on Entities to UniqueKey in DB

                var properties = entityType.GetProperties();
                if ((properties != null) && (properties.Any()))
                {
                    foreach (var property in properties)
                    {
                        foreach (var uniqueKey in GetUniqueKeyAttributes(entityType, property).Where(x => x.Order == 0))
                        {
                            // Single column Unique Key
                            if (uniqueKey.GroupId.IsNullOrWhiteSpace())
                            {
                                entityType.AddIndex(property).IsUnique = true;
                            }
                            // Multiple column Unique Key
                            else
                            {
                                var mutableProperties = new List<IMutableProperty>();
                                properties.ToList().ForEach(x =>
                                {
                                    var uks = GetUniqueKeyAttributes(entityType, x);
                                    if (uks != null)
                                    {
                                        foreach (var uk in uks)
                                        {
                                            if ((uk != null) && (uk.GroupId == uniqueKey.GroupId))
                                            {
                                                mutableProperties.Add(x);
                                            }
                                        }
                                    }
                                });
                                entityType.AddIndex(mutableProperties).IsUnique = true;
                            }
                        }
                    }
                }

                #endregion Convert UniqueKeyAttribute on Entities to UniqueKey in DB
            }

            return modelBuilder;
        }

        private static IEnumerable<UniqueKeyAttribute> GetUniqueKeyAttributes(IMutableEntityType entityType, IMutableProperty property)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if (entityType.ClrType == null)
            {
                throw new NullReferenceException(nameof(entityType.ClrType));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (property.Name == null)
            {
                throw new NullReferenceException(nameof(property.Name));
            }

            var propInfo = entityType.ClrType.GetProperty(
                property.Name,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);

            if (propInfo == null)
            {
                return new UniqueKeyAttribute[0];
            }

            return propInfo.GetCustomAttributes<UniqueKeyAttribute>();
        }
    }
}
