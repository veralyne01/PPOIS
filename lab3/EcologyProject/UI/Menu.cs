
using System;

namespace EcologyProject
{
    public class MenuUI
    {
        private const string HEADER =
            "------------------------------\n" +
            "     ECOSYSTEM SIMULATION\n" +
            "------------------------------\n";

        public static void Menu(Ecosystem ecosystem)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(HEADER);
                Console.WriteLine(
                    "Choose option:\n" +
                    "1. Start new simulation\n" +
                    "2. Show map\n" +
                    "3. Show statistics\n" +
                    "4. Exit\n> "
                );

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ecosystem = new Ecosystem();
                        SuggestCreation(ecosystem);
                        break;

                    case "2":
                        ecosystem.ShowMap();
                        SuggestCreation(ecosystem);
                        break;

                    case "3":
                        Console.WriteLine("\nSTATISTICS:");
                        ecosystem.ShowStatus();
                        Console.ReadLine();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid input! Please, try again");
                        Console.ReadLine();
                        break;
                }
            }
        }

        public static void SuggestCreation(Ecosystem ecosystem)
        {
            while (true)
            {
                Console.WriteLine(
                    "Choose option:\n" +
                    "1. Add new nature area\n" +
                    "2. Add national park\n" + 
                    "3. Add nature reserve\n" +
                    "4. Back\n> "
                );

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddAreaToEcosystem(ecosystem);
                        break;

                    case "2":
                        break;

                    case "3":
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid input! Please, try again");
                        break;
                }
            }
        }

        public static void AddAreaToEcosystem(Ecosystem ecosystem)
        {
            Console.WriteLine(
                "Choose area type:\n" +
                "1. Forest\n" +
                "2. Lake\n" +
                "3. River\n" +
                "4. Sea\n" +
                "5. Mountain\n" +
                "6. Plains\n" +
                "7. Back\n> "
            );

            while (true)
            {
                string? input = Console.ReadLine();

                if (input == "7")
                    return;

                Console.WriteLine("Enter area name:");
                string name = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Enter area size:");
                double size;
                Double.TryParse(Console.ReadLine(), out size);

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name cannot be empty!");
                    return;
                }

                NaturalArea? area = input switch
                {
                    "1" => new Forest(size, name),
                    "2" => new Lake(size, name),
                    "3" => new River(size, name),
                    "4" => new Sea(size, name),
                    "5" => new Mountain(size, name),
                    "6" => new Plains(size, name),
                    _ => null
                };

                if (area == null)
                {
                    Console.WriteLine("Invalid input! Please, try again");
                    continue;
                }

                ecosystem.AddArea(area);

                Console.WriteLine($"{name} successfully added!");
                return;
            }
        }

        public static void AddProtectedArea()
        {

        }

        public static List<NaturalArea> GetAreasAsList(Ecosystem ecosystem)
        {
            int i = 0;
            List<NaturalArea> list = new List<NaturalArea>();
            Dictionary<int, NaturalArea> dict = new Dictionary<int, NaturalArea>();
            foreach (var area in ecosystem.Areas)
            {
                Console.WriteLine($"{++i}. {area.Name}");
                dict.Add(i, area);
            }
            Console.WriteLine("Choose areas to add: ");

            string input = Console.ReadLine();

            string[] parts = input.Split(
                new[] { ' ', ',', ';' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int number))
                {
                    if (dict.TryGetValue(number, out NaturalArea cur)) list.Add(cur);
                }
                else
                {
                    throw new FormatException($"Invalid number: {part}");
                }
            }
            return list;
        }
    }
}
