namespace EcologyProject;

public class Woodpecker : Bird
{
    public Woodpecker(string name = "Woodpecker", IEnumerable<string>? food = null)
        : base(name, BirdTypes.woody, food ?? new[] { "Insect", "Ant", "Stag beetle" }) { }
}
