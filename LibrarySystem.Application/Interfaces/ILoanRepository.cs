using LibrarySystem.Domain;
namespace LibrarySystem.Application.Interfaces;
public interface ILoanRepository
{
    Task AddAsync(
        Loan loan,
        CancellationToken cancellationToken);

    Task<Loan?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}