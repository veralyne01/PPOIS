namespace EcologyProject;

public abstract class Organism
{
    private static int nextId;

    public int Id { get; }
    public string Name { get; }
    public string Type { get; }
    public bool IsAlive { get; private set; } = true;
    public IReadOnlyCollection<string> Food => food;

    protected readonly HashSet<string> food;

    protected Organism(string type, string name, IEnumerable<string>? food = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organism name cannot be empty.", nameof(name));

        Id = Interlocked.Increment(ref nextId);
        Type = type;
        Name = name.Trim();
        this.food = new HashSet<string>(food ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
    }

    public void MarkAsHunted() => MarkEaten();

    internal void MarkEaten()
    {
        if (!IsAlive)
            throw new InvalidOperationException($"{Name} is already dead.");

        IsAlive = false;
    }

    public override string ToString() => $"{Name} ({Type})";
}
