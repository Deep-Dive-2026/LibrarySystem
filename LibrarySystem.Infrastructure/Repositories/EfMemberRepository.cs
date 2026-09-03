using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain;
using LibrarySystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Infrastructure.Repositories;

public class EfMemberRepository : IMemberRepository
{
    private readonly LibraryDbContext _db;

    public EfMemberRepository(
        LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<Member?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _db.Members
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
}