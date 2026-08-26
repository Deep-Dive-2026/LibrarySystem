using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Books.GetBooks;

public class GetBooksHandler
    : IRequestHandler<GetBooksQuery, IReadOnlyList<BookDto>>
{
    private readonly IBookRepository _repository;

    public GetBooksHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BookDto>> Handle(
        GetBooksQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetPageAsync(
            request.Page,
            request.PageSize,
            request.Search,
            cancellationToken);
    }
}
