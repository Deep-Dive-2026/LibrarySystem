namespace LibrarySystem.Domain;

public class Author
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public List<Book> Books { get; private set; } = new();

    private Author() { Name = null!; }

    public Author(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
