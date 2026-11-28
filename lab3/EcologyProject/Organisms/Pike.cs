namespace EcologyProject;

public class Pike : Fish
{
    public Pike(string name = "Pike", IEnumerable<string>? food = null)
        : base(name, FishTypes.freshwater, food ?? new[] { "Crayfish", "Small fish", "Algae" }) { }
}
