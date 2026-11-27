namespace EcologyProject;

public class Eagle : Bird
{
    public Eagle(string name = "Eagle", IEnumerable<string>? food = null)
        : base(name, BirdTypes.predator, food ?? new[] { "Mouse", "Fish", "Hare" }) { }
}
