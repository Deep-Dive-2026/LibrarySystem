using MediatR;
namespace LibrarySystem.Application.Commands.Books.BorrowBook;
public record BorrowBookCommand(
    Guid Id,
    Guid BookId,
    Guid MemberId
) : IRequest<Guid>;