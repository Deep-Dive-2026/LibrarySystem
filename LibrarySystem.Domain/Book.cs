namespace LibrarySystem.Domain;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public Guid? AuthorId { get; private set; }
    public Author? Author { get; private set; }

    private Book() { Title = null!; }

    public Book(Guid id, string title)
    {
        Id = id;
        Title = title;
    }

    public Book(Guid id, string title, Guid authorId)
        : this(id, title)
    {
        AuthorId = authorId;
    }
}
