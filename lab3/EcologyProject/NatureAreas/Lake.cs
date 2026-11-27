namespace EcologyProject;

public class Lake : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Lake;

    public Lake(double size, string name) : base(size, name, new[]
    {
        "Beaver", "Otter", "Muskrat", "Water vole", "Mallard Duck", "Hissing Swan",
        "Grey Heron", "Grebe", "Reed Warbler", "Pike", "Crucian carp", "Perch", "Roach",
        "Bream", "Tench", "Carp", "Walleye", "Lake frog", "Pond frog",
        "Newt", "Marsh turtle", "Common snake", "Crayfish", "Pond crab",
        "Toothless", "Reel", "Dragonfly", "Vodomerka", "Floating beetle", "Mosquito",
        "Water lily", "Cup", "Duckweed", "Reed", "Reed", "Cattail", "Spirogyra", "Chlorella"
    })
    {
        Pollution.Add(PollutionType.Water);
        Pollution.Add(PollutionType.Soil);
    }
}
