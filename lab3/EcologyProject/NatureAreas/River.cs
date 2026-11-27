namespace EcologyProject;

public class River : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.River;

    public River(double size, string name) : base(size, name, new[]
    {
        "Beaver", "Otter", "Muskrat", "Water vole", "Kingfisher", "Grey Heron",
        "Mallard Duck", "Pike", "Perch", "Chub", "Ide", "Asp", "Trout", "Catfish",
        "Walleye", "Lake frog", "Grass frog", "Newt", "Common snake",
        "Marsh turtle", "Crayfish", "Toothless", "Pearl barley", "Coil",
        "Mayfly", "Brook", "Dragonfly", "Water Strider", "Reed", "Reed", "Cattail",
        "Spirogyra", "AquaticPlant"
    })
    {
        Pollution.Add(PollutionType.Water);
        Pollution.Add(PollutionType.Soil);
    }
}
