namespace GeneralizedProgramming
{
    /// <summary>Console menu demonstrating Binary tree sort and MSD radix sort.</summary>
    public sealed class SortingMenu
    {
        private static readonly string[] Names =
            { "Anna", "Boris", "Clara", "Dmitry", "Elena", "Frank", "Gina" };

        private readonly Random _random = new Random();

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("=== Sorting ===");
                Console.WriteLine("1. Binary tree sort");
                Console.WriteLine("2. MSD radix sort");
                Console.WriteLine("0. Back");

                string choice = Prompt("Choice").Trim();
                if (choice == "0") return;
                if (choice != "1" && choice != "2")
                {
                    Console.WriteLine("Unknown menu item.");
                    Console.WriteLine();
                    continue;
                }

                try
                {
                    RunScenario(useRadix: choice == "2");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Input error: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                Console.WriteLine();
            }
        }

        // ---------- Scenario ----------

        private void RunScenario(bool useRadix)
        {
            int type = ReadChoice("Data type", "1 - int, 2 - string, 3 - Student", 1, 3);
            bool asArray = ReadChoice("Container", "1 - array, 2 - List", 1, 2) == 1;
            bool random = ReadChoice("Data source", "1 - enter manually, 2 - generate random", 1, 2) == 2;

            switch (type)
            {
                case 1: SortInts(useRadix, asArray, random); break;
                case 2: SortStrings(useRadix, asArray, random); break;
                default: SortStudents(useRadix, asArray, random); break;
            }
        }

        private void SortInts(bool useRadix, bool asArray, bool random)
        {
            List<int> data = random
                ? GenerateInts(ReadCount())
                : ParseInts(Prompt("Enter integers separated by spaces"));

            if (useRadix)
                RunAndShow(data, asArray, a => MSDRadixSort.Sort(a), l => MSDRadixSort.Sort(l));
            else
                RunAndShow(data, asArray, a => BinaryTreeSort.Sort(a), l => BinaryTreeSort.Sort(l));
        }

        private void SortStrings(bool useRadix, bool asArray, bool random)
        {
            List<string> data = random
                ? GenerateStrings(ReadCount())
                : ParseStrings(Prompt("Enter words separated by spaces"));

            if (useRadix)
                RunAndShow(data, asArray, a => MSDRadixSort.Sort(a), l => MSDRadixSort.Sort(l));
            else
                RunAndShow(data, asArray, a => BinaryTreeSort.Sort(a), l => BinaryTreeSort.Sort(l));
        }

        private void SortStudents(bool useRadix, bool asArray, bool random)
        {
            List<Student> data = random
                ? GenerateStudents(ReadCount())
                : ParseStudents(Prompt("Enter students as Name:Age separated by spaces (e.g. Anna:20 Bob:19)"));

            if (!useRadix)
            {
                Console.WriteLine("Order: by Age, then by Name.");
                RunAndShow(data, asArray, a => BinaryTreeSort.Sort(a), l => BinaryTreeSort.Sort(l));
                return;
            }

            int key = ReadChoice("Radix key", "1 - Age (int key), 2 - Name (string key)", 1, 2);
            if (key == 1)
                RunAndShow(data, asArray, a => MSDRadixSort.Sort(a, s => s.Age), l => MSDRadixSort.Sort(l, s => s.Age));
            else
                RunAndShow(data, asArray, a => MSDRadixSort.Sort(a, s => s.Name), l => MSDRadixSort.Sort(l, s => s.Name));
        }

        /// <summary>Prints the data, sorts a copy (as an array or as a List) and prints the result.</summary>
        private static void RunAndShow<T>(
            List<T> source, bool asArray, Action<T[]> sortArray, Action<List<T>> sortList)
        {
            Console.WriteLine($"Before: {Format(source)}");

            if (asArray)
            {
                T[] array = source.ToArray();
                sortArray(array);
                Console.WriteLine($"After:  {Format(array)}");
            }
            else
            {
                var list = new List<T>(source);
                sortList(list);
                Console.WriteLine($"After:  {Format(list)}");
            }
        }

        private static string Format<T>(IEnumerable<T> items)
        {
            string text = string.Join(", ", items);
            return text.Length == 0 ? "(empty)" : text;
        }

        // ---------- Parsing ----------

        private static string[] Tokens(string input)
            => input.Split(new[] { ' ', ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        private static List<int> ParseInts(string input)
        {
            var result = new List<int>();
            foreach (string token in Tokens(input))
            {
                if (!int.TryParse(token, out int value))
                    throw new FormatException($"'{token}' is not an integer.");
                result.Add(value);
            }
            return result;
        }

        private static List<string> ParseStrings(string input)
            => new List<string>(Tokens(input));

        private static List<Student> ParseStudents(string input)
        {
            var result = new List<Student>();
            foreach (string token in Tokens(input))
            {
                int colon = token.LastIndexOf(':');
                if (colon <= 0 || colon == token.Length - 1)
                    throw new FormatException($"'{token}' does not match the Name:Age format.");

                string name = token.Substring(0, colon);
                if (!int.TryParse(token.Substring(colon + 1), out int age))
                    throw new FormatException($"Age in '{token}' is not an integer.");

                result.Add(new Student(name, age));
            }
            return result;
        }

        // ---------- Random data ----------

        private List<int> GenerateInts(int count)
        {
            var result = new List<int>(count);
            for (int i = 0; i < count; i++) result.Add(_random.Next(-100, 101));
            return result;
        }

        private List<string> GenerateStrings(int count)
        {
            var result = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                int length = _random.Next(1, 7);
                var chars = new char[length];
                for (int j = 0; j < length; j++) chars[j] = (char)('a' + _random.Next(26));
                result.Add(new string(chars));
            }
            return result;
        }

        private List<Student> GenerateStudents(int count)
        {
            var result = new List<Student>(count);
            for (int i = 0; i < count; i++)
                result.Add(new Student(Names[_random.Next(Names.Length)], _random.Next(17, 26)));
            return result;
        }

        // ---------- Input helpers ----------

        private static string Prompt(string text)
        {
            Console.Write($"{text}: ");
            return Console.ReadLine() ?? throw new EndOfStreamException();
        }

        private static int ReadCount()
        {
            while (true)
            {
                string input = Prompt("How many elements (0-1000)");
                if (int.TryParse(input, out int value) && value >= 0 && value <= 1000) return value;
                Console.WriteLine("Enter an integer from 0 to 1000.");
            }
        }

        private static int ReadChoice(string title, string options, int min, int max)
        {
            while (true)
            {
                string input = Prompt($"{title} ({options})");
                if (int.TryParse(input, out int value) && value >= min && value <= max) return value;
                Console.WriteLine($"Enter a number from {min} to {max}.");
            }
        }
    }
}
