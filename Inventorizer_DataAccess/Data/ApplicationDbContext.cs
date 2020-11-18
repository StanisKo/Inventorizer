using Microsoft.EntityFrameworkCore;

using Inventorizer_Models.Models;

namespace Inventorizer_DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<ItemDetail> ItemDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Setup one-to-many between category and item
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.Category_Id);

            // Setup one-to-one between item and item detail
            modelBuilder.Entity<Item>()
                .HasOne(i => i.ItemDetail)
                .WithOne(id => id.Item)
                .HasForeignKey<ItemDetail>(id => id.Item_Id);
        }
    }
}