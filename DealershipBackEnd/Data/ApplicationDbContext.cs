using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DealershipBackEnd.Models;

namespace DealershipBackEnd.Data
{
    /// <summary>
    /// Main database context for the application.
    /// Inherits from IdentityDbContext for user authentication.
    /// Includes DbSets for StockItems, Accessories, and Images.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Table for stock items (cars, vehicles, etc.)
        /// </summary>
        public DbSet<StockItem> StockItems { get; set; }

        /// <summary>
        /// Table for stock accessories linked to StockItems
        /// </summary>
        public DbSet<StockAccessory> StockAccessories { get; set; }

        /// <summary>
        /// Table for images linked to StockItems
        /// </summary>
        public DbSet<Image> Images { get; set; }

        /// <summary>
        /// Configures entity relationships and cascading behaviors.
        /// Sets up:
        /// - StockAccessory -> StockItem (many-to-one)
        /// - Image -> StockItem (many-to-one)
        /// Cascade delete ensures related accessories/images are removed when a StockItem is deleted.
        /// </summary>
        /// <param name="builder">Model builder used to configure EF entities</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure one-to-many relationship for accessories
            builder.Entity<StockAccessory>()
                .HasOne(sa => sa.StockItem)
                .WithMany(si => si.Accessories)
                .HasForeignKey(sa => sa.StockItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship for images
            builder.Entity<Image>()
                .HasOne(i => i.StockItem)
                .WithMany(s => s.Images)
                .HasForeignKey(i => i.StockItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
