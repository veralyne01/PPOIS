namespace EcologyProject;

public class Duck : Bird
{
    public Duck(string name = "Duck", IEnumerable<string>? food = null)
        : base(name, BirdTypes.waterfaul, food ?? new[] { "Algae", "Insect", "Small fish" }) { }
}
