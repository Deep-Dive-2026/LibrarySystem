using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain;

namespace LibrarySystem.Infrastructure.Repositories;

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
}
