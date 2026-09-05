using Hangfire;
using LibrarySystem.Application.Interfaces;

namespace LibrarySystem.Application.Services.Jobs;

public class SendLoanReceiptJob
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IEmailSender _emailSender;

    public SendLoanReceiptJob(
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        IEmailSender emailSender)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _emailSender = emailSender;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync(Guid loanId)
    {
        var cancellationToken = CancellationToken.None;

        var loan = await _loanRepository.GetByIdAsync(
            loanId,
            cancellationToken);

        if (loan is null)
            return;

        var book = await _bookRepository.GetByIdAsync(
            loan.BookId,
            cancellationToken);

        if (book is null)
            return;

        var member = await _memberRepository.GetByIdAsync(
            loan.MemberId,
            cancellationToken);

        if (member is null)
            return;

        var body = $"""
            Hello {member.Name},

            Your borrowing was successful.

            Book: {book.Title}
            Borrowed At: {loan.BorrowedAt:yyyy-MM-dd HH:mm}

            Thank you for using our library.
            """;

        await _emailSender.SendAsync(
            member.Email,
            "Library Borrowing Receipt",
            body,
            cancellationToken);
    }
}