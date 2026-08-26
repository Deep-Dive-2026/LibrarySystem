namespace LibrarySystem.Application.Queries.Books.GetBooks;

public class BookDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? AuthorName { get; init; }
}
