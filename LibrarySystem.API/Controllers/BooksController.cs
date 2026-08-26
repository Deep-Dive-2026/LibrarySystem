using LibrarySystem.Application.Commands.Books.CreateBook;
using LibrarySystem.Application.Queries.Books.GetBook;
using LibrarySystem.Application.Queries.Books.GetBooks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly ISender _sender;

    public BooksController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks(
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 20,
        string? search = null)
    {
        var query = new GetBooksQuery(page, pageSize, search);

        var books = await _sender.Send(query, cancellationToken);

        return Ok(books);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBook(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetBookQuery(id);

        var book = await _sender.Send(query, cancellationToken);

        return book is null
            ? NotFound()
            : Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook(
        CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetBook),
            new { id },
            new { id });
    }
}
