namespace EcologyProject;

public class Factory
{
    private static int nextFactoryId;

    public int Id { get; }
    public string Name { get; }
    public FactoryType Type { get; }
    public NaturalArea Location { get; }

    public Factory(string name, FactoryType type, NaturalArea location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Factory name cannot be empty.", nameof(name));

        Id = Interlocked.Increment(ref nextFactoryId);
        Name = name;
        Type = type;
        Location = location ?? throw new FactoryLocationException();
    }

    public IReadOnlyDictionary<PollutionType, double> GetPollutionImpact() =>
        Type switch
        {
            FactoryType.TPP => new Dictionary<PollutionType, double>
            {
                [PollutionType.Air] = 10,
                [PollutionType.Water] = 3,
                [PollutionType.Soil] = 5
            },
            FactoryType.HPS => new Dictionary<PollutionType, double>
            {
                [PollutionType.Water] = 5
            },
            FactoryType.NPP => new Dictionary<PollutionType, double>
            {
                [PollutionType.Water] = 4,
                [PollutionType.Soil] = 2
            },
            _ => new Dictionary<PollutionType, double>()
        };

    public void Pollute(NaturalArea area, double multiplier = 1)
    {
        if (multiplier < 0 || !double.IsFinite(multiplier))
            throw new InvalidPollutionValueException();

        foreach (var impact in GetPollutionImpact())
            area.Pollution.Increase(impact.Key, impact.Value * multiplier);
    }

    public override string ToString() => $"{Name} ({Type})";
}
