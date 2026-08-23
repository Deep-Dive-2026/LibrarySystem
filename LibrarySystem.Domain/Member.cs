namespace LibrarySystem.Domain;

public class Member
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private Member() { Name = null!; }

    public Member(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
