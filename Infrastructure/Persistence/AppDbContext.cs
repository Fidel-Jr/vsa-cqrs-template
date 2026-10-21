using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VsaCqrsTemplate.Infrastructure.Persistence.Auth;

namespace VsaCqrsTemplate.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SAMPLE of how to apply a global query filter for multi-tenancy

            //modelBuilder.Entity<EntityName>()
            //    .HasQueryFilter(entity => entity.EntityName == _currentTenant.TenantId);
        }
    }
}