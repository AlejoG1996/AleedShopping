using AleedTiendaShopping.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AleedTiendaShopping.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext>options): base(options)
        {
                
        }

        public DbSet<Country> Countries { get; set; }

        protected  override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(c=>c.Name).IsUnique();
        }
    }
    
}
