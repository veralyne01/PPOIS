namespace EcologyProject;

public class Beaver : Mammal
{
    public Beaver(string name = "Beaver", IEnumerable<string>? food = null)
        : base(name, MammalTypes.rodent, food ?? new[] { "Grass", "Reed", "Birch", "Aspen" }) { }
}
