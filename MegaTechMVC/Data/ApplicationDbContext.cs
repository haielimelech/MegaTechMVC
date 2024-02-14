using MegaTechMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace MegaTechMVC.Data

{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Location>().HasKey(location => location.Name);
        }
    }
}

