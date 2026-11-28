namespace EcologyProject;

public class Trout : Fish
{
    public Trout(string name = "Trout", IEnumerable<string>? food = null)
        : base(name, FishTypes.freshwater, food ?? new[] { "Crayfish", "Small fish", "Insect" }) { }
}
