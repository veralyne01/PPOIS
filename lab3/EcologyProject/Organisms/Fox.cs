namespace EcologyProject;

public class Fox : Mammal
{
    public Fox(string name = "Fox", IEnumerable<string>? food = null)
        : base(name, MammalTypes.predator, food ?? new[] { "Hare", "Mouse", "Insect" }) { }
}
