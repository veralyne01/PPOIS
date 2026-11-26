namespace EcologyProject;

public abstract class ProtectedArea
{
    private readonly HashSet<NaturalArea> areas = new();
    public string Name { get; }
    public IReadOnlyCollection<NaturalArea> Areas => areas;
    public abstract double EmissionMultiplier { get; }
    public abstract bool ProtectsAllSpeciesFromHunting { get; }

    protected ProtectedArea(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Protected area name cannot be empty.", nameof(name));
        Name = name;
    }

    public void AddArea(NaturalArea area) => areas.Add(area);
    public bool Contains(NaturalArea area) => areas.Contains(area);
    public abstract bool IsHuntingAllowed(string organismName);
}
