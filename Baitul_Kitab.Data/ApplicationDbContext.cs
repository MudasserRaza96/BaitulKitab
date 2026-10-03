using Baitul_Kitab.Data.Models;
using Baitul_Kitab.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Baitul_Kitab.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly DbContextOptions _options;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { _options = options; }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set precision for Price columns
            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Book>()
                .Property(b => b.DiscountPrice)
                .HasPrecision(10, 2);

            // Book -> Category (nullable FK, no cascade)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Book -> Author (nullable FK, no cascade)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Book -> Language (nullable FK, no cascade)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Language)
                .WithMany()
                .HasForeignKey(b => b.LanguageId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
