using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibrarySystem.Application.Commands.Books.CreateBook;

public class CreateBookHandler
    : IRequestHandler<CreateBookCommand, Guid>
{
    private readonly IBookRepository _repository;
    private readonly ILogger<CreateBookHandler> _logger;

    public CreateBookHandler(
        IBookRepository repository,
        ILogger<CreateBookHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Guid> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = new Book(
            request.Id,
            request.Title);

        await _repository.AddAsync(
            book,
            cancellationToken);

        // business event, structured properties
        _logger.LogInformation(
            "Book {BookId} created with title {Title}",
            book.Id, book.Title);

        return book.Id;
    }
}
