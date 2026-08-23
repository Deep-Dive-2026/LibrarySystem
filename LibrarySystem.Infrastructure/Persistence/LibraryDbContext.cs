using LibrarySystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Infrastructure.Persistence;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(
        DbContextOptions<LibraryDbContext> options)
        : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // best practice: one IEntityTypeConfiguration per entity,
        // discovered automatically from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LibraryDbContext).Assembly);
    }
}
