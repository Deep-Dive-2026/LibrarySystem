using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain;
using LibrarySystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Infrastructure.Repositories;

public class EfBookRepository : IBookRepository
{
    private readonly LibraryDbContext _db;

    public EfBookRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<Book?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _db.Books
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Book book,
        CancellationToken cancellationToken)
    {
        _db.Books.Add(book);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
