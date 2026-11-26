namespace EcologyProject;

public class Bird : Animal
{
    public BirdTypes Category { get; }

    public Bird(string name, BirdTypes category, IEnumerable<string>? food = null)
        : base("Bird", name, food ?? Array.Empty<string>())
    {
        Category = category;
    }

    public Bird(string name, IEnumerable<string>? food = null)
        : this(name, BirdTypes.passeriformes, food)
    {
    }
}
