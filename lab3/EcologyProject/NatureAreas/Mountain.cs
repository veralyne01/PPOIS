namespace EcologyProject;

public class Mountain : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Mountain;

    public Mountain(double size, string name) : base(size, name, new[]
    {
        "Brown bear", "Wolf", "Fox", "Lynx", "Red deer", "Mountain goat",
        "Snow Leopard", "Alpine Marmot", "Argali", "Chamois", "Yak", "Snow vole",
        "Golden Eagle", "Bearded", "Alpine Jackdaw", "Ular", "Mountain bumblebee",
        "Alpine butterfly", "Mountain viper", "Rock lizard", "Alpine newt",
        "Salamander", "Edelweiss", "Gentian", "Alpine poppy", "Mountain Pine",
        "Juniper", "Mosses", "Lichens", "Trout"
    })
    {
        Pollution.Add(PollutionType.Air);
        Pollution.Add(PollutionType.Soil);
    }
}
