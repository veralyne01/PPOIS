namespace EcologyProject;

public class WildlifeSanctuary : ProtectedArea
{
    private readonly HashSet<string> protectedSpecies;

    public override double EmissionMultiplier => 0.5;
    public override bool ProtectsAllSpeciesFromHunting => false;

    public IReadOnlyCollection<string> ProtectedSpecies => protectedSpecies;

    public WildlifeSanctuary(string name, IEnumerable<string> protectedSpecies, List<NaturalArea> areas) : base(name, areas)
    {
        this.protectedSpecies = new HashSet<string>(
            protectedSpecies ?? throw new ArgumentNullException(nameof(protectedSpecies)),
            StringComparer.OrdinalIgnoreCase);

        if (this.protectedSpecies.Count == 0)
            throw new EmptySanctuaryException(nameof(protectedSpecies));
    }

    public override bool IsHuntingAllowed(string organismName) =>
        !protectedSpecies.Contains(organismName);
}
