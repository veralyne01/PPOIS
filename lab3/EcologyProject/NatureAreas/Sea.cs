namespace EcologyProject;

public class Sea : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Sea;

    public Sea(double size, string name) : base(size, name, new[]
    {
        "Dolphin", "Blue Whale", "Killer whale", "Seal", "Walrus", "Albatross",
        "Seagull", "Cormorant", "Penguin", "Petrel", "Shark", "Tuna", "Cod",
        "Flounder", "Mackerel", "Seahorse", "Turtle",
        "Kamchatka Crab", "Lobster", "Shrimp", "Lobster",
        "Octopus", "Squid", "Mussel", "Oyster", "Scallop", "Jellyfish",
        "Sea Anemone", "Coral", "Laminaria", "Fucus", "Ulva", "Zostera"
    })
    {
        Pollution.Add(PollutionType.Water);
    }
}
