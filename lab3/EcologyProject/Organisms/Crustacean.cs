namespace EcologyProject;

public class Crustacean : Animal
{
    public Crustacean(string name, IEnumerable<string>? food = null)
        : base("Crustacean", name, food ?? Array.Empty<string>())
    {
    }
}
