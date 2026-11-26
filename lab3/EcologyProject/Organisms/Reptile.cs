namespace EcologyProject;

public class Reptile : Animal
{
    public Reptile(string name, IEnumerable<string>? food = null)
        : base("Reptile", name, food ?? Array.Empty<string>())
    {
    }
}
