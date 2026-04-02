using BookLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BookLibrary.Data;

public class BookLibraryContext : DbContext
{
    public BookLibraryContext(DbContextOptions<BookLibraryContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BookLoan> BookLoans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
            entity.Property(b => b.ISBN).HasMaxLength(20);
            entity.Property(b => b.Genre).HasMaxLength(50);
            entity.HasIndex(b => b.ISBN).IsUnique();
            entity.HasIndex(b => b.Genre);
            entity.HasIndex(b => b.Author);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<BookLoan>(entity =>
        {
            entity.HasKey(bl => bl.Id);
            entity.HasOne(bl => bl.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(bl => bl.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(bl => bl.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(bl => bl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(bl => new { bl.BookId, bl.IsReturned });
            entity.HasIndex(bl => bl.UserId);
        });
    }
}