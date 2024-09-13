using Microsoft.EntityFrameworkCore;
using HealthInsurance.Entities;

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
        public DbSet<CompanyDetails> CompanyDetails { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<HospitalInfo> HospitalInfo { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.CompanyDetails)
                .WithMany()
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior as needed

            modelBuilder.Entity<Policy>()
                .HasOne(p => p.HospitalInfo)
                .WithMany()
                .HasForeignKey(p => p.MedicalId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior as needed

            // Additional configurations
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
