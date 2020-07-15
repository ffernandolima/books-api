using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace BooksApi.Models.MySQL
{
    public class Context : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public Context(DbContextOptions<Context> options)
           : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.BookInfo).HasJsonConversion();
                entity.ToTable("Books");
            });

            var books = new Book[]
            {
                new Book
                {
                    Id = 1,
                    BookInfo = new BookInfo
                    {
                        BookName = "Book test 1",
                        Author = "Fernando",
                        Category = "Finance",
                        CreatedAt = DateTime.UtcNow,
                        Price = 100
                    }
                },

                new Book
                {
                    Id = 2,
                    BookInfo = new BookInfo
                    {
                        BookName = "Book test 2",
                        Author = "Matheus",
                        Category = "Fiction",
                        CreatedAt = DateTime.UtcNow,
                        Price = 200
                    }
                },

                new Book
                {
                    Id = 3,
                    BookInfo = new BookInfo
                    {
                        BookName = "Book test 3",
                        Author = "Pedro",
                        Category = "Self Help",
                        CreatedAt = DateTime.UtcNow,
                        Price = 300
                    }
                }
            };

            builder.Entity<Book>().HasData(books);
        }
    }
}
