namespace EcologyProject;

public class PollutionLevel
{
    private readonly Dictionary<PollutionType, double> levels = new();

    public IReadOnlyDictionary<PollutionType, double> Levels => levels;

    public void Add(PollutionType type, double value = 0)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        levels[type] = value;
    }

    public bool Increase(PollutionType type, double value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        if (!levels.ContainsKey(type))
            return false;

        levels[type] += value;
        return true;
    }

    public bool Decrease(PollutionType type, double value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        if (!levels.ContainsKey(type))
            return false;

        levels[type] = Math.Max(0, levels[type] - value);
        return true;
    }

    public double? GetLevel(PollutionType type) =>
        levels.TryGetValue(type, out double value) ? value : null;

    public bool HasParameter(PollutionType type) => levels.ContainsKey(type);
}
