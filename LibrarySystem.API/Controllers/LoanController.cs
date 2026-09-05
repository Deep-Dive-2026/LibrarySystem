using LibrarySystem.Application.Commands.Books.BorrowBook;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ISender _sender;

    public LoansController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Borrow(
        BorrowBookCommand command,
        CancellationToken cancellationToken)
    {
        var loanId = await _sender.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(Borrow),
            new { id = loanId },
            new { id = loanId });
    }
}