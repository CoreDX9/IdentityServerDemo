using Microsoft.EntityFrameworkCore;
using Skoruba.AuditLogging.EntityFramework.DbContexts;
using Skoruba.AuditLogging.EntityFramework.Entities;
using System.Threading.Tasks;

namespace CoreDX.Application.EntityFrameworkCore.IdentityServer.Admin
{
    public class AdminAuditLogDbContext : DbContext, IAuditLoggingDbContext<AuditLog>
    {
        public AdminAuditLogDbContext(DbContextOptions<AdminAuditLogDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public DbSet<AuditLog> AuditLog { get; set; }
    }
}






