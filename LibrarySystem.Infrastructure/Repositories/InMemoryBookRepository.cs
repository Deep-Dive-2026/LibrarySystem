using LibrarySystem.Application.Interfaces;
using LibrarySystem.Application.Queries.Books.GetBooks;
using LibrarySystem.Application.Queries.Books.GetTopBooks;
using LibrarySystem.Domain;

namespace LibrarySystem.Infrastructure.Repositories;

// Session 1 artifact — replaced by EfBookRepository in Session 2.
// Kept as the before/after teaching example.
public class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> _books = new();

    public Task<Book?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var book = _books.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(book);
    }

    public Task AddAsync(
        Book book,
        CancellationToken cancellationToken)
    {
        _books.Add(book);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<BookDto>> GetPageAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<BookDto> result = _books
            .Where(b => string.IsNullOrWhiteSpace(search)
                     || b.Title.Contains(search))
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title
            })
            .ToList();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<TopBooksDto>> GetTopBooksAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
