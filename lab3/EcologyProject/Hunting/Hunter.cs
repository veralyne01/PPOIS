namespace EcologyProject;

public class Hunter
{
    private readonly Dictionary<string, int> target;
    private readonly Dictionary<string, int> killed = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, int> Target => target;
    public IReadOnlyDictionary<string, int> Killed => killed;

    public Hunter(Dictionary<string, int> target)
    {
        this.target = target ?? throw new ArgumentNullException(nameof(target));
    }

    public void Kill(Organism victim, NaturalArea area, Ecosystem ecosystem)
    {
        if (!victim.IsAlive || !target.TryGetValue(victim.Name, out int limit))
            throw new WrongVictimException();

        int alreadyKilled = killed.GetValueOrDefault(victim.Name);
        if (alreadyKilled >= limit || !ecosystem.CanHunt(victim.Name, area))
            throw new WrongVictimException();

        victim.MarkAsHunted();
        area.RemoveCreature(victim);
        killed[victim.Name] = alreadyKilled + 1;
    }
}
