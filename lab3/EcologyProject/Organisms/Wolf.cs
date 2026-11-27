namespace EcologyProject;

public class Wolf : Mammal
{
    public Wolf(string name = "Wolf", IEnumerable<string>? food = null)
        : base(name, MammalTypes.predator, food ?? new[] { "Hare", "Deer", "Squirrel", "Grass" }) { }
}
