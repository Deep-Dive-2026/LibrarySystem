using System.Diagnostics;
using LibrarySystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.API.Controllers;

// Session 2 live-demo endpoints — throwaway teaching code.
// Each endpoint exists to be clicked in Swagger while the SQL log
// (and SSMS / Extended Events) is open. Compare shapes, count queries.
[ApiController]
[Route("demos")]
public class EfDemosController : ControllerBase
{
    private readonly LibraryDbContext _db;

    public EfDemosController(LibraryDbContext db)
    {
        _db = db;
    }

    // N+1: 1 query for books + 1 query PER book. Watch the log storm.
    [HttpGet("nplus1")]
    public async Task<IActionResult> NPlusOne(CancellationToken ct)
    {
        var books = await _db.Books
            .TagWith("DEMO nplus1 – base query")
            .Take(100)
            .ToListAsync(ct);

        var titles = new List<string>();

        foreach (var book in books)
        {
            // one round trip per book — the N in N+1
            var author = await _db.Authors
                .TagWith("DEMO nplus1 – per-book author lookup")
                .FirstOrDefaultAsync(a => a.Id == book.AuthorId, ct);

            titles.Add($"{book.Title} — {author?.Name}");
        }

        return Ok(titles);
    }

    // Fix A: Include — one JOIN, one round trip.
    [HttpGet("include")]
    public async Task<IActionResult> Include(CancellationToken ct)
    {
        var books = await _db.Books
            .TagWith("DEMO include – single JOIN")
            .Include(b => b.Author)
            .Take(100)
            .ToListAsync(ct);

        return Ok(books.Select(b => $"{b.Title} — {b.Author?.Name}"));
    }

    // Fix B: projection — 3 columns, no entities, no tracking.
    [HttpGet("projection")]
    public async Task<IActionResult> Projection(CancellationToken ct)
    {
        var books = await _db.Books
            .TagWith("DEMO projection – select only what we need")
            .Take(100)
            .Select(b => new
            {
                b.Id,
                b.Title,
                AuthorName = b.Author!.Name
            })
            .ToListAsync(ct);

        return Ok(books);
    }

    // Cartesian explosion vs AsSplitQuery: ?split=true to compare.
    [HttpGet("author-graph")]
    public async Task<IActionResult> AuthorGraph(
        bool split,
        CancellationToken ct)
    {
        var query = _db.Authors
            .TagWith($"DEMO author-graph – split={split}")
            .Include(a => a.Books)
            .Take(5);

        if (split) query = query.AsSplitQuery();

        var authors = await query.ToListAsync(ct);

        return Ok(authors.Select(a => new
        {
            a.Name,
            Books = a.Books.Count
        }));
    }

    // Tracking overhead: ?track=false to skip the change tracker.
    [HttpGet("tracking")]
    public async Task<IActionResult> Tracking(
        bool track,
        CancellationToken ct)
    {
        IQueryable<Domain.Book> query = _db.Books
            .TagWith($"DEMO tracking – track={track}")
            .Take(10_000);

        if (!track) query = query.AsNoTracking();

        var books = await query.ToListAsync(ct);

        return Ok(new
        {
            Loaded  = books.Count,
            Tracked = _db.ChangeTracker.Entries().Count()
        });
    }

    // The 4-cell matrix: before/after × small/large — one endpoint, two knobs.
    //   ?optimized=false&rows=100      before · small  → feels fine
    //   ?optimized=true&rows=100       after  · small  → same result
    //   ?optimized=false&rows=100000   before · large  → the trap revealed
    //   ?optimized=true&rows=100000    after  · large  → still fast
    [HttpGet("matrix")]
    public async Task<IActionResult> Matrix(
        bool optimized,
        int rows,
        CancellationToken ct,
        string search = "chair")
    {
        var sw = Stopwatch.StartNew();
        int loadedIntoMemory;
        List<string> page;

        if (!optimized)
        {
            // BEFORE — the IEnumerable world.
            // ToListAsync() ends the SQL story: everything after it
            // runs in this process, on every fetched row.
            var all = await _db.Books
                .TagWith($"MATRIX before – rows={rows}")
                .Include(b => b.Author)          // all columns, twice
                .Take(rows)                      // simulated table size
                .ToListAsync(ct);                // ← work happens HERE

            loadedIntoMemory = all.Count;

            page = all                           // LINQ-to-Objects from here
                // gotcha: C# Contains is case-sensitive by default,
                // SQL LIKE usually isn't — the two worlds differ in
                // SEMANTICS too, not just performance
                .Where(b => b.Title.Contains(
                    search, StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.Title)
                .Take(20)
                .Select(b => $"{b.Title} — {b.Author?.Name}")
                .ToList();
        }
        else
        {
            // AFTER — the IQueryable world.
            // The whole chain compiles into ONE SQL statement;
            // only 20 projected rows ever cross the wire.
            page = await _db.Books
                .TagWith($"MATRIX after – rows={rows}")
                .Take(rows)                      // same simulated table size
                .Where(b => b.Title.Contains(search))
                .OrderBy(b => b.Title)
                .Take(20)
                .Select(b => b.Title + " — " + b.Author!.Name)
                .ToListAsync(ct);                // ← single SQL statement

            loadedIntoMemory = page.Count;
        }

        sw.Stop();

        return Ok(new
        {
            optimized,
            simulatedTableRows = rows,
            rowsLoadedIntoMemory = loadedIntoMemory,
            rowsServed = page.Count,
            elapsedMs = sw.ElapsedMilliseconds
        });
    }

    // Set-based write: bulk-close ancient open loans without loading them.
    [HttpPost("bulk-return")]
    public async Task<IActionResult> BulkReturn(CancellationToken ct)
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);

        var affected = await _db.Loans
            .Where(l => l.ReturnedAt == null
                     && l.BorrowedAt < cutoff)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(l => l.ReturnedAt, DateTime.UtcNow), ct);

        return Ok(new { affected });
    }
}
