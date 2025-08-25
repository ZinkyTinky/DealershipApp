using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DealershipBackEnd.Models;

namespace DealershipBackEnd.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<StockAccessory> StockAccessories { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relationships
            builder.Entity<StockAccessory>()
                .HasOne(sa => sa.StockItem)
                .WithMany(si => si.Accessories)
                .HasForeignKey(sa => sa.StockItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Image>()
                .HasOne(i => i.StockItem)
                .WithMany(s => s.Images)
                .HasForeignKey(i => i.StockItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
