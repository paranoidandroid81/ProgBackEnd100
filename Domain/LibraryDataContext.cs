using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Domain
{
    public class LibraryDataContext : DbContext
    {
        public LibraryDataContext(DbContextOptions<LibraryDataContext> ctx) : base(ctx)
        {
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().Property(p => p.Title).HasMaxLength(200);
            modelBuilder.Entity<Book>().Property(p => p.Author).HasMaxLength(200);

            modelBuilder.Entity<Book>()
                .HasData(
                new Book { Id = 1, Title = "Walden", Author = "Threau", Genre = "Philosophy", NumberOfPages = 322 },
                new Book { Id = 2, Title = "Rhythm Science", Author = "DJ Spooky That Subliminal Kid", Genre = "Music", NumberOfPages = 180 },
                new Book { Id = 3, Title = "Nature", Author = "Emerson", Genre = "Philosophy", NumberOfPages = 182 }
                );
        }
    }
}
