namespace EcologyProject;

public class Mammal : Animal
{
    public MammalTypes Category { get; }

    public Mammal(string name, MammalTypes category, IEnumerable<string>? food = null)
        : base("Mammal", name, food ?? Array.Empty<string>())
    {
        Category = category;
    }

    public Mammal(string name, IEnumerable<string>? food = null)
        : this(name, MammalTypes.predator, food)
    {
    }
}
