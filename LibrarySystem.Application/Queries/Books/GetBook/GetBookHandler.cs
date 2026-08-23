using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain;
using MediatR;

namespace LibrarySystem.Application.Queries.Books.GetBook;

public class GetBookHandler
    : IRequestHandler<GetBookQuery, Book?>
{
    private readonly IBookRepository _repository;

    public GetBookHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Book?> Handle(
        GetBookQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);
    }
}
