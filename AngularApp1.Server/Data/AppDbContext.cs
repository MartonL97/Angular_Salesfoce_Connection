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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        
        }
    }
}
