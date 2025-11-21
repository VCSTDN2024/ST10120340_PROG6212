using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing
        public DbSet<LecturerClaim> LecturerClaims { get; set; }

        // PART 3 ADDITIONS
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure LecturerClaim
            modelBuilder.Entity<LecturerClaim>()
                .Property(lc => lc.HoursWorked)
                .HasPrecision(18, 2);

            modelBuilder.Entity<LecturerClaim>()
                .Property(lc => lc.HourlyRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<LecturerClaim>()
                .Property(lc => lc.TotalAmount)
                .HasPrecision(18, 2);

            // Configure Lecturer
            modelBuilder.Entity<Lecturer>()
                .Property(l => l.DefaultHourlyRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Lecturer>()
                .HasIndex(l => l.Email)
                .IsUnique();

            // Configure Invoice
            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TaxAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.NetAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();
        }
    }
}