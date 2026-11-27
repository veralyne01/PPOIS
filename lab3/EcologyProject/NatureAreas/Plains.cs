namespace EcologyProject;

public class Plains : NaturalArea
{
    public override NaturalAreaType AreaType => NaturalAreaType.Plains;

    public Plains(double size, string name) : base(size, name, new[]
    {
        "Wolf", "Fox", "Common fox", "Hare", "Hare-hare", "Gopher",
        "Groundhog", "Saiga", "Bison", "Roe deer", "Korsak", "Steppe ferret", "Vole",
        "Steppe Eagle", "Lark", "Quail", "Bustard", "Grey partridge",
        "Beauty Crane", "Locust", "Grasshopper", "Mantis", "Bumblebee",
        "Steppe viper", "Nimble lizard", "Yellow-bellied runner", "Tarantula",
        "Karakurt", "Kovyl", "Tipchak", "Wormwood", "Clover", "Chamomile", "Cornflower", "Poppy"
    })
    {
        Pollution.Add(PollutionType.Air);
        Pollution.Add(PollutionType.Soil);
    }
}
