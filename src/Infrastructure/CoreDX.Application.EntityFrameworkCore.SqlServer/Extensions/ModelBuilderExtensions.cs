using CoreDX.Domain.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CoreDX.Application.EntityFrameworkCore.SqlServer.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// 配置插入顺序记录接口属性为自增列（通常为使用非自增主键（例如Guid）时用于保存记录的插入顺序）
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="builder">实体类型构造器</param>
        public static void ConfigForIStorageOrderRecordable<TEntity>(this EntityTypeBuilder<TEntity> builder)
      where TEntity : class, IStorageOrderRecordable
        {
            builder.Property(e => e.InsertOrder).Metadata
                .SetValueGenerationStrategy(SqlServerValueGenerationStrategy.IdentityColumn);
        }

        /// <summary>
        /// 配置插入顺序记录接口属性为聚集索引（通常为使用非自增主键（例如Guid）时用于避免非自增主键作为聚集索引引起的性能问题）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="builder"></param>
        public static void ConfigClusteredIndexForIStorageOrderRecordable<TEntity>(this EntityTypeBuilder<TEntity> builder)
where TEntity : class, IStorageOrderRecordable
        {
            builder.HasIndex(e => e.InsertOrder).IsUnique().IsClustered();
        }
    }
}
