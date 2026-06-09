namespace Core;

public class Account
{
    private readonly List<Credit> _credits = [];

    public required Guid Id { init; get; }
    public required string Name { set; get; }
    public required DateTime CreatedAt { set; get; }
    public required IReadOnlyCollection<Credit> Credits { init => _credits = [.. value]; get => _credits; }

    public void Transfer(Account recipient, Guid creditId)
    {
    }

    public void Create(Project project, Credit credit)
    {
    }
}
