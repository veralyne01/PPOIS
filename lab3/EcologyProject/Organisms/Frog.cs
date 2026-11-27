namespace EcologyProject;

public class Frog : Amphibian
{
    public Frog(string name = "Frog", IEnumerable<string>? food = null)
        : base(name, food ?? new[] { "Insect", "Mosquito", "Dragonfly" }) { }
}
