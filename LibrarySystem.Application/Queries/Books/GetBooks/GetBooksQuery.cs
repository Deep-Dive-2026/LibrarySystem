using MediatR;

namespace LibrarySystem.Application.Queries.Books.GetBooks;

public record GetBooksQuery(
    int Page,
    int PageSize,
    string? Search
) : IRequest<IReadOnlyList<BookDto>>;
