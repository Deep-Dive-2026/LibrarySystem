using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain;

namespace LibrarySystem.Application.Services;

public class BookService
{
    private readonly IBookRepository _repository;

    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Book?> GetBookAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }
}
