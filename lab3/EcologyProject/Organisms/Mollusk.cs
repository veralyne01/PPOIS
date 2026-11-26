namespace EcologyProject;

public class Mollusk : Animal
{
    public Mollusk(string name, IEnumerable<string>? food = null)
        : base("Mollusk", name, food ?? Array.Empty<string>())
    {
    }
}
