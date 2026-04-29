using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Models;

namespace Shop.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<DataRentApart> apartmentsavito { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<TaskApp> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataRentApart>().HasNoKey();
        }
    }
}
