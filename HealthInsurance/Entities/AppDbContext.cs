using Microsoft.EntityFrameworkCore;
using HealthInsurance.Entities; // Ensure you include this namespace

namespace HealthInsurance.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets for each entity
        public DbSet<AdminAccount> Admin { get; set; }
        public DbSet<EmpRegister> EmpRegisteration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // You can add any custom configurations here if needed
            // Example of configuring entity properties
            modelBuilder.Entity<AdminAccount>()
                .Property(a => a.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<EmpRegister>()
                .Property(e => e.Salary)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
