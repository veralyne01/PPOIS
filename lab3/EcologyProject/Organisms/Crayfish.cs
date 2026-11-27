namespace EcologyProject;

public class Crayfish : Crustacean
{
    public Crayfish(string name = "Crayfish", IEnumerable<string>? food = null)
        : base(name, food ?? new[] { "Algae", "Spirogyra" }) { }
}
