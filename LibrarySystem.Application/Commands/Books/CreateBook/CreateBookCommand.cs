using MediatR;

namespace LibrarySystem.Application.Commands.Books.CreateBook;

public record CreateBookCommand(
    Guid Id,
    string Title
) : IRequest<Guid>;
