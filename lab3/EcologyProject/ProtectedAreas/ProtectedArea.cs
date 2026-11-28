namespace EcologyProject;

public abstract class ProtectedArea
{
    private readonly List<NaturalArea> areas = new();
    public string Name { get; }
    public IReadOnlyCollection<NaturalArea> Areas => areas;
    public abstract double EmissionMultiplier { get; }
    public abstract bool ProtectsAllSpeciesFromHunting { get; }

    protected ProtectedArea(string name, List<NaturalArea> areas)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidProtectedAreaException();
        if (areas is null || areas.Count == 0) throw new InvalidProtectedAreaException();
        Name = name;
        this.areas.AddRange(areas);
    }

    public void AddArea(NaturalArea area)
    {
        ArgumentNullException.ThrowIfNull(area);
        if (!areas.Contains(area)) areas.Add(area);
    }
    public bool Contains(NaturalArea area) => areas.Contains(area);
    public abstract bool IsHuntingAllowed(string organismName);
}
