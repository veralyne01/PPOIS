using EcologyProject.Exceptions;

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
            throw new ObjectIsNotInTheEcosystemException();
        factories.Add(factory);
    }

    public void AddProtectedArea(ProtectedArea protectedArea, IEnumerable<NaturalArea> protectedZones)
    {
        ArgumentNullException.ThrowIfNull(protectedArea);
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
        Console.WriteLine($"Natural areas: {areas.Count}; factories: {factories.Count}; protected areas: {protectedAreas.Count}");

        foreach (var area in areas)
        {
            Console.WriteLine();
            Console.WriteLine($"{area.Name} ({area.AreaType}), {area.Size:F1} km²");
            Console.WriteLine($"Organisms: {area.Organisms.Count(item => item.IsAlive)}");
            foreach (var organism in area.Organisms.Where(item => item.IsAlive))
            {
                if (organism is Animal animal)
                    Console.WriteLine($"  {organism.Name} [{organism.Type}], hunger: {animal.Hunger}/100");
                else
                    Console.WriteLine($"  {organism.Name} [{organism.Type}]");
            }
            foreach (var pollution in area.Pollution.Levels)
                Console.WriteLine($"  {pollution.Key}: {pollution.Value:F2}");
        }
    }
}
