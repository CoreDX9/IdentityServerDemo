using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class Domain
    {
        public int Id { get; set; }

        public int C1 { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Domain> Domains { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
