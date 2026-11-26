namespace EcologyProject;

public abstract class NaturalArea
{
    private readonly HashSet<string> allowedOrganisms;
    private readonly List<Organism> organisms = new();
    private readonly HashSet<NaturalArea> neighbors = new();

    public int Id { get; }
    private static int nextId;
    public double Size { get; }
    public string Name { get; }
    public abstract NaturalAreaType AreaType { get; }
    public PollutionLevel Pollution { get; } = new();
    public IReadOnlyList<Organism> Organisms => organisms;
    public IReadOnlyCollection<NaturalArea> Neighbors => neighbors;
    public int MapX { get; internal set; } = -1;
    public int MapY { get; internal set; } = -1;
    public AreaClimate? Climate { get; set; }

    protected NaturalArea(double size, string name, IEnumerable<string> allowedNames)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Area name cannot be empty.", nameof(name));

        Id = Interlocked.Increment(ref nextId);
        Size = size;
        Name = name;
        allowedOrganisms = new HashSet<string>(allowedNames, StringComparer.OrdinalIgnoreCase);
    }

    public bool CanContainOrganism(Organism organism) =>
        allowedOrganisms.Contains(organism.Name);

    public void AddCreature(Organism organism)
    {
        ArgumentNullException.ThrowIfNull(organism);

        if (!organism.IsAlive)
            throw new InvalidOperationException("A dead organism cannot be added to an area.");

        if (!CanContainOrganism(organism))
            throw new UnsuitableNatureAreaException();

        if (organisms.Any(item => item.Id == organism.Id))
            throw new InvalidOperationException("This organism is already in the area.");

        organisms.Add(organism);
    }

    public bool RemoveCreature(Organism organism) => organisms.Remove(organism);

    public void AddNeighbor(NaturalArea area)
    {
        if (area == this)
            return;

        neighbors.Add(area);
        area.neighbors.Add(this);
    }

    internal void RemoveDeadOrganisms() => organisms.RemoveAll(item => !item.IsAlive);

    public override string ToString() => $"{Name} [{AreaType}]";
}
