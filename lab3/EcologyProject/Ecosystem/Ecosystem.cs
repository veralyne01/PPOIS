namespace EcologyProject;

public class Ecosystem
{
    private readonly List<NaturalArea> areas = new();
    private readonly List<Factory> factories = new();
    private readonly List<ProtectedArea> protectedAreas = new();
    public IReadOnlyList<NaturalArea> Areas => areas;
    public IReadOnlyList<Factory> Factories => factories;
    public IReadOnlyList<ProtectedArea> ProtectedAreas => protectedAreas;

    public Ecosystem()
    {
        Console.WriteLine("New ecosystem created!\n");
    }

    public void AddArea(NaturalArea area, int x = -1, int y = -1)
    {
        ArgumentNullException.ThrowIfNull(area);
        if (areas.Contains(area))
            throw new InvalidOperationException("This area is already in the ecosystem.");

        if (x < 0 || y < 0)
        {
            int position = areas.Count;
            y = position / 4;
            int column = position % 4;
            x = y % 2 == 0 ? column : 3 - column;

            while (areas.Any(item => item.MapX == x && item.MapY == y))
            {
                position++;
                y = position / 4;
                column = position % 4;
                x = y % 2 == 0 ? column : 3 - column;
            }
        }
        else if (areas.Any(item => item.MapX == x && item.MapY == y))
        {
            throw new MapException();
        }

        area.MapX = x;
        area.MapY = y;
        areas.Add(area);
        area.Climate ??= CreateDefaultClimate(area.AreaType);
        SeedArea(area);

        foreach (var neighbor in areas.Where(item =>
            item != area && Math.Abs(item.MapX - x) + Math.Abs(item.MapY - y) == 1))
        {
            area.AddNeighbor(neighbor);
        }
    }

    public void ConnectAreas(NaturalArea first, NaturalArea second)
    {
        if (!areas.Contains(first) || !areas.Contains(second))
            throw new ObjectIsNotInTheEcosystemException();

        first.AddNeighbor(second);
    }

    public void AddFactory(Factory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (!areas.Contains(factory.Location))
            throw new FactoryLocationException();
        if (factories.Any(item => item.Id == factory.Id))
            throw new InvalidOperationException("This factory is already registered.");
        factories.Add(factory);
    }

    public void AddProtectedArea(ProtectedArea protectedArea, IEnumerable<NaturalArea> protectedZones)
    {
        ArgumentNullException.ThrowIfNull(protectedArea);
        ArgumentNullException.ThrowIfNull(protectedZones);
        if (!protectedZones.Any() || protectedAreas.Any(item => item.Name.Equals(protectedArea.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidProtectedAreaException();
        foreach (var area in protectedZones)
        {
            if (!areas.Contains(area))
                throw new ObjectIsNotInTheEcosystemException();
            protectedArea.AddArea(area);
        }
        protectedAreas.Add(protectedArea);
    }

    public bool CanHunt(string organismName, NaturalArea area)
    {
        return !protectedAreas.Any(item =>
            item.Contains(area) && !item.IsHuntingAllowed(organismName));
    }

    public void ApplyFactoryEmissions()
    {
        foreach (var factory in factories)
        {
            var affectedAreas = new HashSet<NaturalArea> { factory.Location };
            foreach (var neighbor in factory.Location.Neighbors)
                affectedAreas.Add(neighbor);

            foreach (var area in affectedAreas)
            {
                double multiplier = 1;
                foreach (var protectedArea in protectedAreas.Where(item => item.Contains(area)))
                    multiplier = Math.Min(multiplier, protectedArea.EmissionMultiplier);

                factory.Pollute(area, multiplier);
            }
        }
    }



    public int Day { get; private set; }
    public const double PollutionLimit = 70;

    public void AdvanceTime(int steps)
    {
        if (steps < 1 || steps > 36500) throw new InvalidSimulationStepException();
        for (int i = 0; i < steps; i++) AdvanceOneDay();
    }

    public void AdvanceOneDay()
    {
        Day++;
        GenerateFactories();
        ApplyFactoryEmissions();

        foreach (var area in areas)
        {
            area.CurrentWeather = new TodayWeather(area.Climate ?? CreateDefaultClimate(area.AreaType));
            if (area.CurrentWeather.Temperature < -5 || area.CurrentWeather.Temperature > 32)
            {
                foreach (var animal in area.Organisms.OfType<Animal>().Where(item => item.IsAlive))
                    animal.IncreaseHunger(10);
            }
            PopulateArea(area);
            var animals = area.Organisms.OfType<Animal>().Where(item => item.IsAlive).ToList();
            foreach (var animal in animals) animal.LiveDay(area);
            ReproduceAnimals(area);
            ApplyPollutionDamage(area);
            area.RemoveDeadOrganisms();
        }
    }

    private static AreaClimate CreateDefaultClimate(NaturalAreaType type) => type switch
    {
        NaturalAreaType.Forest => new AreaClimate(-25, 30, 20, 90, 10, 100),
        NaturalAreaType.Lake => new AreaClimate(-15, 28, 30, 100, 20, 100),
        NaturalAreaType.River => new AreaClimate(-15, 30, 20, 100, 10, 100),
        NaturalAreaType.Sea => new AreaClimate(-2, 30, 20, 100, 20, 100),
        NaturalAreaType.Mountain => new AreaClimate(-30, 18, 10, 90, 10, 100),
        _ => new AreaClimate(-20, 35, 0, 70, 0, 90)
    };

    private void SeedArea(NaturalArea area)
    {
        string[] names = area.AllowedOrganismNames.ToArray();
        if (names.Length == 0) return;

        string[] plants = names.Where(OrganismCatalog.IsPlant).ToArray();
        string[] animals = names.Where(name => !OrganismCatalog.IsPlant(name) && !OrganismCatalog.IsMushroom(name)).ToArray();
        string[] stableSpecies = animals.Where(name => !OrganismCatalog.IsPredator(name)).ToArray();
        var selected = new List<string>();
        if (plants.Length > 0) selected.Add(plants[Random.Shared.Next(plants.Length)]);
        if (animals.Length > 0)
        {
            string[] candidates = stableSpecies.Length > 0 ? stableSpecies : animals;
            string initialSpecies = candidates[Random.Shared.Next(candidates.Length)];
            selected.Add(initialSpecies);
            selected.Add(initialSpecies);
        }
        foreach (string name in selected)
            area.AddCreature(OrganismCatalog.Create(name, names));
    }

    private void GenerateFactories()
    {
        if (areas.Count == 0 || Random.Shared.NextDouble() >= 0.001) return;
        var location = areas[Random.Shared.Next(areas.Count)];
        var type = (FactoryType)Random.Shared.Next(Enum.GetValues<FactoryType>().Length);
        var factory = new Factory($"Factory-{factories.Count + 1}", type, location);
        AddFactory(factory);
        Console.WriteLine($"A new {type} factory appeared near {location.Name}.");
    }

    private void PopulateArea(NaturalArea area)
    {
        RegrowPlants(area);
        IntroduceAnimals(area);
    }
    private void ReproduceAnimals(NaturalArea area)
    {
        int capacity = Math.Max(6, (int)(area.Size / 5));
        int living = area.Organisms.Count(item => item.IsAlive);
        if (living >= capacity) return;

        var groups = area.Organisms.OfType<Animal>().Where(item => item.IsAlive)
            .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var group in groups)
        {
            var parents = group.Where(item => item.CanReproduce).ToList();
            if (parents.Count < 2 || Random.Shared.NextDouble() >= 0.08) continue;

            string species = group.Key;
            var offspring = OrganismCatalog.Create(species, area.AllowedOrganismNames);
            area.AddCreature(offspring);
            parents[0].MarkReproduced();
            parents[1].MarkReproduced();
            living++;
            Console.WriteLine($"A new {species} was born in {area.Name}.");
            if (living >= capacity) break;
        }
    }

    private static void ApplyPollutionDamage(NaturalArea area)
    {
        if (area.Pollution.Levels.Count == 0) return;
        double pollution = area.Pollution.Levels.Values.Average();
        if (pollution <= PollutionLimit) return;

        double deathChance = Math.Min(0.75, (pollution - PollutionLimit) / 120.0);
        foreach (var organism in area.Organisms.Where(item => item.IsAlive).ToList())
            if (Random.Shared.NextDouble() < deathChance) organism.MarkEaten();
    }

    public void ShowMap()
    {
        if (areas.Count == 0)
        {
            Console.WriteLine("The ecosystem has no natural areas yet.");
            return;
        }

        int maxX = areas.Max(item => item.MapX);
        int maxY = areas.Max(item => item.MapY);
        var map = areas.ToDictionary(item => (item.MapX, item.MapY));

        Console.WriteLine();
        Console.WriteLine("MAP:");
        Console.WriteLine("Legend: F forest | L lake | R river | M mountain | P plains | S sea");
        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                if (map.TryGetValue((x, y), out var area))
                {
                    char symbol = area.AreaType switch
                    {
                        NaturalAreaType.Forest => 'F',
                        NaturalAreaType.Lake => 'L',
                        NaturalAreaType.River => 'R',
                        NaturalAreaType.Mountain => 'M',
                        NaturalAreaType.Plains => 'P',
                        NaturalAreaType.Sea => 'S',
                        _ => '?'
                    };
                    Console.Write($"[{symbol}:{area.Name[..Math.Min(5, area.Name.Length)]}]");
                }
                else
                {
                    Console.Write("[     ]");
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine();
        foreach (var area in areas)
        {
            string neighbors = area.Neighbors.Count == 0
                ? "none"
                : string.Join(", ", area.Neighbors.Select(item => item.Name));
            Console.WriteLine($"{area.Name}: neighbors — {neighbors}");
        }
    }

    public void ShowStatus()
    {
        Console.WriteLine($"Day: {Day}");
        Console.WriteLine($"Natural areas: {areas.Count}; factories: {factories.Count}; protected areas: {protectedAreas.Count}");

        foreach (var factory in factories)
            Console.WriteLine($"Factory: {factory.Name}, {factory.Type}, location: {factory.Location.Name}");

        foreach (var area in areas)
        {
            Console.WriteLine();
            Console.WriteLine($"{area.Name} ({area.AreaType}), {area.Size:F1} km²");
            Console.WriteLine($"Organisms: {area.Organisms.Count(item => item.IsAlive)}");
            if (area.CurrentWeather is not null) area.CurrentWeather.ShowCurrentWeather();
            foreach (var organism in area.Organisms.Where(item => item.IsAlive))
            {
                if (organism is Animal animal)
                    Console.WriteLine($"  {organism.Name} [{organism.Type}], age: {animal.AgeDays}/{animal.LifespanDays} days, hunger: {animal.Hunger}/100");
                else
                    Console.WriteLine($"  {organism.Name} [{organism.Type}]");
            }
            foreach (var pollution in area.Pollution.Levels)
                Console.WriteLine($"  {pollution.Key}: {pollution.Value:F2}");
        }
    }

    private void RegrowPlants(NaturalArea area)
    {
        string[] plants = area.AllowedOrganismNames
            .Where(OrganismCatalog.IsPlant)
            .ToArray();

        if (plants.Length == 0) return;

        int plantCount = area.Organisms
            .Count(item => item.IsAlive && item is Plant);

        int capacity = Math.Max(4, (int)(area.Size / 4));

        if (plantCount >= capacity || Random.Shared.NextDouble() >= 0.35)
            return;

        string name = plants[Random.Shared.Next(plants.Length)];
        area.AddCreature(OrganismCatalog.Create(name, area.AllowedOrganismNames));

        Console.WriteLine($"New {name} appeared in {area.Name}.");
    }

    private void IntroduceAnimals(NaturalArea area)
    {
        int capacity = Math.Max(4, (int)(area.Size / 10));

        int living = area.Organisms
            .OfType<Animal>()
            .Count(item => item.IsAlive);

        if (living >= capacity) return;
        string[] species = area.AllowedOrganismNames
            .Where(name => !OrganismCatalog.IsPlant(name) &&
                           !OrganismCatalog.IsMushroom(name))
            .ToArray();

        if (species.Length == 0) return;

        int animalCount = area.Organisms
            .OfType<Animal>()
            .Count(item => item.IsAlive);

        if (animalCount >= capacity || Random.Shared.NextDouble() >= 0.03)
            return;

        foreach (string name in species.OrderBy(_ => Random.Shared.Next()))
        {
            var animal = OrganismCatalog.Create(name, area.AllowedOrganismNames);

            bool hasFood = animal.Food.Any(foodName =>
                area.Organisms.Any(item =>
                    item.IsAlive &&
                    item.Name.Equals(foodName, StringComparison.OrdinalIgnoreCase)));

            if (!hasFood) continue;

            area.AddCreature(animal);

            Console.WriteLine($"A new {name} appeared in {area.Name}.");
            return;
        }
    }
}
