namespace EcologyProject;

public class Insect : Animal
{
    public Insect(string name, IEnumerable<string>? food = null)
        : base("Insect", name, food ?? Array.Empty<string>())
    {
    }
}
