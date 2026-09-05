using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain;
using LibrarySystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Infrastructure.Repositories;

public class EfLoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _db;

    public EfLoanRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(
        Loan loan,
        CancellationToken cancellationToken)
    {
        await _db.Loans.AddAsync(
            loan,
            cancellationToken);

        await _db.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<Loan?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _db.Loans
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
}