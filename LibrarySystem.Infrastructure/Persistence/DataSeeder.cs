using Bogus;
using LibrarySystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Infrastructure.Persistence;

public static class DataSeeder
{
    // tune live in session — small enough to seed fast,
    // large enough to make bad queries hurt
    private const int AuthorCount = 5_000;
    private const int BookCount = 100_000;
    private const int MemberCount = 20_000;
    private const int LoanCount = 500_000;
    private const int BatchSize = 10_000;

    public static async Task SeedAsync(
        LibraryDbContext db,
        CancellationToken cancellationToken = default)
    {
        if (await db.Books.AnyAsync(cancellationToken)) return;

        db.ChangeTracker.AutoDetectChangesEnabled = false;

        var faker = new Faker();

        var authors = Enumerable.Range(0, AuthorCount)
            .Select(_ => new Author(faker.Random.Guid(), faker.Name.FullName()))
            .ToList();

        var books = Enumerable.Range(0, BookCount)
            .Select(_ => new Book(
                faker.Random.Guid(),
                faker.Commerce.ProductName(),
                faker.PickRandom(authors).Id))
            .ToList();

        var members = Enumerable.Range(0, MemberCount)
            .Select(_ => new Member(faker.Random.Guid(), faker.Name.FullName()))
            .ToList();

        var loans = Enumerable.Range(0, LoanCount)
            .Select(_ => new Loan(
                faker.Random.Guid(),
                faker.PickRandom(books).Id,
                faker.PickRandom(members).Id,
                faker.Date.Past(2)))
            .ToList();

        await InsertInBatchesAsync(db, authors, cancellationToken);
        await InsertInBatchesAsync(db, books, cancellationToken);
        await InsertInBatchesAsync(db, members, cancellationToken);
        await InsertInBatchesAsync(db, loans, cancellationToken);

        db.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    private static async Task InsertInBatchesAsync<T>(
        LibraryDbContext db,
        List<T> items,
        CancellationToken cancellationToken) where T : class
    {
        foreach (var batch in items.Chunk(BatchSize))
        {
            db.AddRange(batch);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
        }
    }
}
