using AllupMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AllupMVC.DAL
{
    public class AppDBContext : IdentityDbContext<AppUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Slide> Slides { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> Images { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTags> ProductTags { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<BasketItems> BasketItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
