using BookLendingSystem.Domain.Entities;
using BookLendingSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookLendingSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Loan>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .IsRequired();

            builder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany()
                .HasForeignKey(l => l.BookId)
                .IsRequired();

            
            builder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();
        }
    }
}
