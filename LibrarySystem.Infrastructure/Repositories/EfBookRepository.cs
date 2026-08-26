using LibrarySystem.Application.Interfaces;
using LibrarySystem.Application.Queries.Books.GetBooks;
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

    // Session 2: filter in SQL → stable order → page → project → materialize last
    public async Task<IReadOnlyList<BookDto>> GetPageAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        IQueryable<Book> query = _db.Books;

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Title.Contains(search));

        return await query
            .TagWith("GetBooks – catalog page")
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookDto
            {
                Id         = b.Id,
                Title      = b.Title,
                AuthorName = b.Author!.Name
            })
            .ToListAsync(cancellationToken);
    }
}
