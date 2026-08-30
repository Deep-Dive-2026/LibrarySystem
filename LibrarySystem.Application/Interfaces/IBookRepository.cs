using LibrarySystem.Application.Queries.Books.GetBooks;
using LibrarySystem.Application.Queries.Books.GetTopBooks;
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
    Task<IReadOnlyList<TopBooksDto>> GetTopBooksAsync(
        CancellationToken cancellationToken);

}
