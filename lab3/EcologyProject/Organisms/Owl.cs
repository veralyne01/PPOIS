namespace EcologyProject;

public class Owl : Bird
{
    public Owl(string name = "Owl", IEnumerable<string>? food = null)
        : base(name, BirdTypes.predator, food ?? new[] { "Mouse", "Insect", "Vole" }) { }
}
