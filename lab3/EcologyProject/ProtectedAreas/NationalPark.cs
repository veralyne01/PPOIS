namespace EcologyProject;

public class WildlifeSanctuary : ProtectedArea
{
    private readonly HashSet<string> protectedSpecies;

    public override double EmissionMultiplier => 0.5;
    public override bool ProtectsAllSpeciesFromHunting => false;

    public IReadOnlyCollection<string> ProtectedSpecies => protectedSpecies;

    public WildlifeSanctuary(string name, IEnumerable<string> protectedSpecies) : base(name)
    {
        this.protectedSpecies = new HashSet<string>(
            protectedSpecies ?? throw new ArgumentNullException(nameof(protectedSpecies)),
            StringComparer.OrdinalIgnoreCase);

        if (this.protectedSpecies.Count == 0)
            throw new ArgumentException("Sanctuary must protect at least one species.", nameof(protectedSpecies));
    }

    public override bool IsHuntingAllowed(string organismName) =>
        !protectedSpecies.Contains(organismName);
}
