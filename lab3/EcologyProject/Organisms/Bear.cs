namespace EcologyProject;

public class Bear : Mammal
{
    public Bear(string name = "Bear", IEnumerable<string>? food = null)
        : base(name, MammalTypes.predator, food ?? new[] { "Hare", "Deer", "Squirrel", "Grass" }) { }
}
