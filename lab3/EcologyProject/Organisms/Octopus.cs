namespace EcologyProject;

public class Octopus : Mollusk
{
    public Octopus(string name = "Octopus", IEnumerable<string>? food = null)
        : base(name, food ?? new[] { "Algae", "Small fish" }) { }
}
