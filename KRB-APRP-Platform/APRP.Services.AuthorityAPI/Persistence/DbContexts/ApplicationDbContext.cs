using APRP.Services.AuthorityAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace APRP.Services.AuthorityAPI.Persistence.DbContexts
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {

        }
      
        public DbSet<Authority> Authorities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authority>().ToTable("Authorities");
        }

    }
}
