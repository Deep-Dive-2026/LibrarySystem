using LibrarySystem.Application.Queries.Books.GetBooks;
using LibrarySystem.Domain;

namespace LibrarySystem.Application.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task AddAsync(
        Book book,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<BookDto>> GetPageAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);
}
