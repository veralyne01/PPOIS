namespace EcologyProject;

public class Fish : Animal
{
    public FishTypes Category { get; }

    public Fish(string name, FishTypes category, IEnumerable<string>? food = null)
        : base("Fish", name, food ?? Array.Empty<string>())
    {
        Category = category;
    }

    public Fish(string name, IEnumerable<string>? food = null)
        : this(name, FishTypes.freshwater, food)
    {
    }
}
