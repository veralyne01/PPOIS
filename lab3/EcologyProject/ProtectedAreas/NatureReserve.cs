namespace EcologyProject;

public class NatureReserve : ProtectedArea
{
    public override double EmissionMultiplier => 0;
    public override bool ProtectsAllSpeciesFromHunting => true;

    public NatureReserve(string name, List<NaturalArea> areas) : base(name, areas)
    {
    }

    public override bool IsHuntingAllowed(string organismName) => false;
}
