namespace EcologyProject;

public abstract class Animal : Organism
{
    public const int HungerThreshold = 60;
    public const int FoodValue = 65;
    public int Hunger { get; private set; }
    public int AgeDays { get; private set; }
    public int LifespanDays { get; }
    public int ReproductionCooldown { get; private set; }
    public bool IsMature => AgeDays >= 30;

    protected Animal(string type, string name, IEnumerable<string> food)
        : base(type, name, food)
    {
        LifespanDays = Random.Shared.Next(1200, 9000);
    }

    public void IncreaseHunger(int amount = 20)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Hunger = Math.Min(100, Hunger + amount);
    }

    public bool Eat(Organism prey)
    {
        if (!IsAlive) throw new OrganismAlreadyDeadException();
        if (Hunger < HungerThreshold) return false;
        if (ReferenceEquals(this, prey) || !prey.IsAlive ||
            (prey is not Animal && prey is not Plant && prey is not Mushroom) ||
            !food.Contains(prey.Name))
            throw new WrongVictimException();

        prey.MarkEaten();
        Hunger = Math.Max(0, Hunger - FoodValue);
        return true;
    }

    public bool TryEat(NaturalArea area)
    {
        if (!IsAlive) throw new OrganismAlreadyDeadException();
        if (Hunger < HungerThreshold) return false;

        Organism? prey = area.Organisms.FirstOrDefault(item =>
            item.IsAlive && !ReferenceEquals(item, this) && food.Contains(item.Name));
        if (prey is null) throw new InsufficientFoodException();
        return Eat(prey);
    }

    public void LiveDay(NaturalArea area)
    {
        if (!IsAlive) return;
        AgeDays++;
        if (ReproductionCooldown > 0) ReproductionCooldown--;
        if (AgeDays >= LifespanDays)
        {
            MarkEaten();
            return;
        }

        IncreaseHunger();
        if (Hunger >= HungerThreshold)
        {
            try { TryEat(area); }
            catch (InsufficientFoodException) { }
        }
        if (Hunger >= 100) MarkEaten();
    }

    public bool CanReproduce => IsAlive && IsMature && Hunger < 50 && ReproductionCooldown == 0;

    public void MarkReproduced()
    {
        if (!CanReproduce) throw new InvalidOperationException("Animal is not ready to reproduce.");
        ReproductionCooldown = 30;
    }
}
