namespace EcologyProject;

public class Forest : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Forest;

    public Forest(double size, string name) : base(size, name, new[]
    {
        "Bear", "Wolf", "Fox", "Lynx",
        "Hog", "Squirrel", "Hedgehog", "Moose", "Deer", "Badger", "Hare",
        "Woodpecker", "Owl", "Cuckoo", "Titmouse", "Capercaillie", "Ant", "Stag beetle",
        "Common viper", "Viviparous lizard", "Grass frog", "Gray Toad",
        "Cross spider", "Birch", "Oak", "Pine", "Spruce", "Maple", "Aspen",
        "Lily", "Fern", "Blueberry", "Lingonberry", "White mushroom", "Birch ",
        "Chanterelle", "Fly Agaric", "Cladonia", "Parmelia"
    })
    {
        Pollution.Add(PollutionType.Air);
        Pollution.Add(PollutionType.Soil);
    }
}
