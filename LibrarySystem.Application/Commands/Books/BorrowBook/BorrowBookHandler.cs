using Hangfire;
using LibrarySystem.Application.Commands.Books.BorrowBook;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Application.Services.Jobs;
using LibrarySystem.Domain;
using MediatR;

namespace LibrarySystem.Application.Commands.Loans.BorrowBook;

public class BorrowBookHandler
    : IRequestHandler<BorrowBookCommand, Guid>
{
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;
    public BorrowBookHandler(
        IBookRepository bookRepository,
        ILoanRepository loanRepository,
        IBackgroundJobClient backgroundJobClient)
    {
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Guid> Handle(
        BorrowBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(
            request.BookId,
            cancellationToken);

        if (book is null)
            throw new InvalidOperationException(
                "Book not found.");

        var loan = new Loan(
            request.Id,
            request.BookId,
            request.MemberId,
            DateTime.UtcNow);

        await _loanRepository.AddAsync(
            loan,
            cancellationToken);

        _backgroundJobClient.Enqueue<SendLoanReceiptJob>(
            job => job.ExecuteAsync(loan.Id));

        return loan.Id;
    }
}