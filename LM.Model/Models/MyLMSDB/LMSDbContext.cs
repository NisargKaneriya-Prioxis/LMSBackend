using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LM.Model.Models.MyLMSDB;

public partial class LMSDbContext : DbContext
{
    public LMSDbContext(DbContextOptions<LMSDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BorrowedBook> BorrowedBooks { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<RequestBook> RequestBooks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Book__3DE0C22736A1C6AD");

            entity.Property(e => e.BorrowedStatus).HasDefaultValue(4);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Book_Category");
        });

        modelBuilder.Entity<BorrowedBook>(entity =>
        {
            entity.HasKey(e => e.BorrowedId).HasName("PK__Borrowed__565E30CC4E72C391");

            entity.Property(e => e.BorrowedBookStatus).HasDefaultValue(9);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Book).WithMany(p => p.BorrowedBooks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BorrowedBook_Book");

            entity.HasOne(d => d.User).WithMany(p => p.BorrowedBooks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BorrowedBook_User");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B7D8B6489");
        });

        modelBuilder.Entity<RequestBook>(entity =>
        {
            entity.HasKey(e => e.RequestBookId).HasName("PK__RequestB__67FB096138930A77");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RequestBookStatus).HasDefaultValue(11);
            entity.Property(e => e.RequestDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Book).WithMany(p => p.RequestBooks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RequestBo__BookI__16CE6296");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RequestBookCreatedByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RequestBo__Creat__17C286CF");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.RequestBookModifiedByNavigations).HasConstraintName("FK__RequestBo__Modif__18B6AB08");

            entity.HasOne(d => d.User).WithMany(p => p.RequestBookUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RequestBo__UserI__15DA3E5D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC3E05D885");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValue("Student");
            entity.Property(e => e.Status).HasDefaultValue(1);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
