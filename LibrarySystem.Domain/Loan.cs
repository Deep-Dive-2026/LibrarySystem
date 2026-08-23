namespace LibrarySystem.Domain;

public class Loan
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public Guid MemberId { get; private set; }
    public DateTime BorrowedAt { get; private set; }
    public DateTime? ReturnedAt { get; private set; }

    private Loan() { }

    public Loan(Guid id, Guid bookId, Guid memberId, DateTime borrowedAt)
    {
        Id = id;
        BookId = bookId;
        MemberId = memberId;
        BorrowedAt = borrowedAt;
    }

    public void Return(DateTime returnedAt)
    {
        ReturnedAt = returnedAt;
    }
}
