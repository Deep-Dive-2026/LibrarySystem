using LibrarySystem.Domain;

namespace LibrarySystem.Application.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}