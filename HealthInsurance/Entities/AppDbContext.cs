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

            // Configuration for AdminAccount Role
            modelBuilder.Entity<AdminAccount>()
                .Property(a => a.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Configuration for EmpRegister Salary
            modelBuilder.Entity<EmpRegister>()
                .Property(e => e.Salary)
                .HasColumnType("decimal(18, 2)");

            // Foreign Key Relationship for Policy and CompanyDetails
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.CompanyDetails)
                .WithMany(c => c.Policy)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign Key Relationship for Policy and HospitalInfo
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.HospitalInfo)
                .WithMany(h => h.Policy)
                .HasForeignKey(p => p.MedicalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
