namespace EcologyProject;

public class Amphibian : Animal
{
    public Amphibian(string name, IEnumerable<string>? food = null)
        : base("Amphibian", name, food ?? Array.Empty<string>())
    {
    }
}
