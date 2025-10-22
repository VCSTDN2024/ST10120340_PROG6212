using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LecturerClaim> LecturerClaims { get; set; }
    }
}
