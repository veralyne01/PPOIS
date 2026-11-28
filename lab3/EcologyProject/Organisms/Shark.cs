namespace EcologyProject;

public class Shark : Fish
{
    public Shark(string name = "Shark", IEnumerable<string>? food = null)
        : base(name, FishTypes.marine, food ?? new[] { "Crab", "Small fish", "Squid" }) { }
}
