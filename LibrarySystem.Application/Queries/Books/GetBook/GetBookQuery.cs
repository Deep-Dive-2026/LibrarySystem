using LibrarySystem.Domain;
using MediatR;

namespace LibrarySystem.Application.Queries.Books.GetBook;

public record GetBookQuery(
    Guid Id
) : IRequest<Book?>;
