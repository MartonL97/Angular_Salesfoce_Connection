using AngularApp1.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace AngularApp1.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().HasData(
                new Player { Id = 1, UserName = "ali3lwa16@gmail.com", Password = "Rsi2025" }
            );
        }
    }
}
