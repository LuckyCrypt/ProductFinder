using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;

namespace Shop.Domain
{
    public class DBContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        public DBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>(e =>
            {
                e.HasIndex(c => c.Slug).IsUnique();
                e.HasOne(c => c.Parent)
                    .WithMany(c => c.Children)
                    .HasForeignKey(c => c.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Product>(e =>
            {
                e.Property(p => p.PriceMin).HasColumnType("numeric(12,2)");
                e.Property(p => p.PriceMax).HasColumnType("numeric(12,2)");
                e.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ProductSpecification>(e =>
            {
                e.HasOne(s => s.Product)
                    .WithMany(p => p.Specifications)
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Store>(e =>
            {
                e.Property(s => s.Code).HasMaxLength(32);
                e.HasIndex(s => s.Code);
            });

            builder.Entity<Offer>(e =>
            {
                e.Property(o => o.Price).HasColumnType("numeric(12,2)");
                e.Property(o => o.LastStatus).HasMaxLength(32);
                e.HasOne(o => o.Product)
                    .WithMany(p => p.Offers)
                    .HasForeignKey(o => o.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(o => o.Store)
                    .WithMany(s => s.Offers)
                    .HasForeignKey(o => o.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
                // Один магазин — одно предложение на товар.
                e.HasIndex(o => new { o.ProductId, o.StoreId }).IsUnique();
            });

            builder.Entity<Favorite>(e =>
            {
                e.HasOne(f => f.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(f => f.Product)
                    .WithMany()
                    .HasForeignKey(f => f.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(f => new { f.UserId, f.ProductId }).IsUnique();
            });
        }
    }
}
