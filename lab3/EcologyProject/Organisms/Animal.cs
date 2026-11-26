namespace EcologyProject;

public abstract class Animal : Organism
{
    public const int HungerThreshold = 70;
    public const int FoodValue = 55;

    public int Hunger { get; private set; }

    protected Animal(string type, string name, IEnumerable<string> food)
        : base(type, name, food)
    {
    }

    public void IncreaseHunger(int amount = 25)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Hunger = Math.Min(100, Hunger + amount);
    }

    public bool Eat(Organism prey)
    {
        if (!IsAlive)
            throw new InvalidOperationException($"{Name} cannot eat because it is dead.");

        if (Hunger < HungerThreshold)
            return false;

        if (ReferenceEquals(this, prey) || !prey.IsAlive ||
            (prey is not Animal && prey is not Plant) || !food.Contains(prey.Name))
            throw new WrongVictimException();

        prey.MarkEaten();
        Hunger = Math.Max(0, Hunger - FoodValue);
        return true;
    }

    public bool TryEat(NaturalArea area)
    {
        if (Hunger < HungerThreshold || !IsAlive)
            return false;

        Organism? prey = area.Organisms.FirstOrDefault(
            organism => organism.IsAlive && food.Contains(organism.Name));

        if (prey is null)
            return false;

        return Eat(prey);
    }

    public void LiveDay(NaturalArea area)
    {
        IncreaseHunger();
        TryEat(area);
    }
}
