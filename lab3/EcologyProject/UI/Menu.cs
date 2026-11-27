using System.Globalization;

namespace EcologyProject;

public class MenuUI
{
    private const string Header =
        "------------------------------\n" +
        "     ECOSYSTEM SIMULATION\n" +
        "------------------------------\n";

    public static void Menu(Ecosystem ecosystem)
    {
        if (ecosystem.Areas.Count == 0)
        {
            Console.WriteLine("Create the first natural area to start the ecosystem.");
            AddAreaToEcosystem(ecosystem);
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(Header);
            Console.WriteLine(
                "1. Add natural area\n" +
                "2. Show map\n" +
                "3. Show ecosystem statistics\n" +
                "4. Live one day\n" +
                "5. Live several days\n" +
                "6. Add protected area\n" +
                "7. Organize hunting\n" +
                "8. Create new ecosystem\n" +
                "9. Exit\n> ");

            string? input = Console.ReadLine();
            try
            {
                switch (input)
                {
                    case "1":
                        AddAreaToEcosystem(ecosystem);
                        break;
                    case "2":
                        ecosystem.ShowMap();
                        Pause();
                        break;
                    case "3":
                        ecosystem.ShowStatus();
                        Pause();
                        break;
                    case "4":
                        ecosystem.AdvanceOneDay();
                        Console.WriteLine($"One day has passed. Current day: {ecosystem.Day}.");
                        ecosystem.ShowStatus();
                        Pause();
                        break;
                    case "5":
                        AdvanceSimulation(ecosystem);
                        break;
                    case "6":
                        Console.WriteLine("1. Wildlife sanctuary\n2. Nature reserve\n> ");
                        string? protectedType = Console.ReadLine();
                        if (protectedType == "1") AddProtectedArea(ecosystem, false);
                        else if (protectedType == "2") AddProtectedArea(ecosystem, true);
                        else Console.WriteLine("Invalid protected area type.");
                        break;
                    case "7":
                        OrganizeHunt(ecosystem);
                        break;
                    case "8":
                        ecosystem = new Ecosystem();
                        Console.WriteLine("Add natural areas to the new ecosystem.");
                        AddAreaToEcosystem(ecosystem);
                        break;
                    case "9":
                        return;
                    default:
                        Console.WriteLine("Invalid menu option.");
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Operation failed: {exception.Message}");
                Pause();
            }
        }
    }

    public static void SuggestCreation(Ecosystem ecosystem)
    {
        while (true)
        {
            Console.WriteLine("1. Add natural area\n2. Add wildlife sanctuary\n3. Add nature reserve\n4. Back\n> ");
            switch (Console.ReadLine())
            {
                case "1": AddAreaToEcosystem(ecosystem); break;
                case "2": AddProtectedArea(ecosystem, false); break;
                case "3": AddProtectedArea(ecosystem, true); break;
                case "4": return;
                default: Console.WriteLine("Invalid input."); break;
            }
        }
    }

    public static void AddAreaToEcosystem(Ecosystem ecosystem)
    {
        Console.WriteLine("Choose area type:\n1. Forest\n2. Lake\n3. River\n4. Sea\n5. Mountain\n6. Plains\n7. Back\n> ");
        string? type = Console.ReadLine();
        if (type == "7") return;

        Console.Write("Enter area name: ");
        string name = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter area size in km²: ");
        string sizeInput = Console.ReadLine() ?? string.Empty;
        if (!double.TryParse(sizeInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double size) &&
            !double.TryParse(sizeInput, out size))
        {
            Console.WriteLine("Invalid area size.");
            return;
        }

        if (string.IsNullOrWhiteSpace(name) || size <= 0)
        {
            Console.WriteLine("Area name must not be empty and size must be positive.");
            return;
        }

        NaturalArea? area = type switch
        {
            "1" => new Forest(size, name),
            "2" => new Lake(size, name),
            "3" => new River(size, name),
            "4" => new Sea(size, name),
            "5" => new Mountain(size, name),
            "6" => new Plains(size, name),
            _ => null
        };

        if (area is null)
        {
            Console.WriteLine("Invalid area type.");
            return;
        }

        ecosystem.AddArea(area);
        Console.WriteLine($"{area.Name} added. Initial organisms have settled in the area.");
        ecosystem.ShowStatus();
    }

    public static void AddProtectedArea(Ecosystem ecosystem, bool reserve)
    {
        if (ecosystem.Areas.Count == 0)
        {
            Console.WriteLine("Add natural areas first.");
            return;
        }

        Console.Write("Enter protected area name: ");
        string name = Console.ReadLine() ?? string.Empty;
        List<NaturalArea> selectedAreas = GetAreasAsList(ecosystem);
        if (selectedAreas.Count == 0) return;

        ProtectedArea protectedArea;
        if (reserve)
        {
            protectedArea = new NatureReserve(name, selectedAreas);
        }
        else
        {
            Console.Write("Enter protected species separated by commas: ");
            string[] species = (Console.ReadLine() ?? string.Empty)
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            protectedArea = new WildlifeSanctuary(name, species, selectedAreas);
        }

        ecosystem.AddProtectedArea(protectedArea, selectedAreas);
        Console.WriteLine($"{name} successfully created.");
    }

    public static void OrganizeHunt(Ecosystem ecosystem)
    {
        if (ecosystem.Areas.Count == 0) return;
        Console.WriteLine("Choose the hunting area:");
        List<NaturalArea> selected = GetAreasAsList(ecosystem);
        if (selected.Count == 0) return;
        NaturalArea area = selected[0];
        Console.Write("Enter the species to hunt: ");
        string species = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter the maximum number of animals: ");
        if (string.IsNullOrWhiteSpace(species) || !int.TryParse(Console.ReadLine(), out int limit) || limit < 1)
        {
            Console.WriteLine("Invalid hunting request.");
            return;
        }

        var hunter = new Hunter(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            [species] = limit
        });
        var victims = area.Organisms.Where(item => item is Animal && item.IsAlive && item.Name.Equals(species, StringComparison.OrdinalIgnoreCase))
            .Take(limit).ToList();
        foreach (var victim in victims)
        {
            try
            {
                hunter.Kill(victim, area, ecosystem);
                Console.WriteLine($"{victim.Name} was hunted.");
            }
            catch (WrongVictimException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        if (victims.Count == 0) Console.WriteLine("No animals of this species were found.");
    }

    public static void AdvanceSimulation(Ecosystem ecosystem)
    {
        Console.Write("How many days should pass? ");
        if (!int.TryParse(Console.ReadLine(), out int days) || days < 1 || days > 36500)
        {
            Console.WriteLine("Enter a whole number from 1 to 36500.");
            return;
        }

        ecosystem.AdvanceTime(days);
        Console.WriteLine($"{days} day(s) passed. Current day: {ecosystem.Day}.");
        ecosystem.ShowStatus();
        Pause();
    }

    public static List<NaturalArea> GetAreasAsList(Ecosystem ecosystem)
    {
        for (int i = 0; i < ecosystem.Areas.Count; i++)
            Console.WriteLine($"{i + 1}. {ecosystem.Areas[i].Name}");
        Console.Write("Select area numbers separated by commas: ");
        string[] parts = (Console.ReadLine() ?? string.Empty)
            .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        var selected = new List<NaturalArea>();
        foreach (string part in parts)
        {
            if (!int.TryParse(part, out int number) || number < 1 || number > ecosystem.Areas.Count)
                throw new FormatException($"Invalid area number: {part}");
            NaturalArea area = ecosystem.Areas[number - 1];
            if (!selected.Contains(area)) selected.Add(area);
        }
        return selected;
    }

    private static void Pause()
    {
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }
}
