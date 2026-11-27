namespace EcologyProject;

public class Deer : Mammal
{
    public Deer(string name = "Deer", IEnumerable<string>? food = null)
        : base(name, MammalTypes.artiodactyl, food ?? new[] { "Grass", "Shrub", "Oak", "Birch", "Clover" }) { }
}
