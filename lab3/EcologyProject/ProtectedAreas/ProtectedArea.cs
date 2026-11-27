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
        Name = name;
        this.areas = areas;
    }

    public void AddArea(NaturalArea area) => areas.Add(area);
    public bool Contains(NaturalArea area) => areas.Contains(area);
    public abstract bool IsHuntingAllowed(string organismName);
}
