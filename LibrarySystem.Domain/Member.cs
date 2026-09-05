namespace LibrarySystem.Domain;

public class Member
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    private Member()
    {
        Name = null!;
        Email = null!;
    }
    public Member(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}
