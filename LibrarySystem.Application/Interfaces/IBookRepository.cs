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
}
