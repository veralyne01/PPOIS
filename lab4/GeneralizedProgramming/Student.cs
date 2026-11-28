namespace GeneralizedProgramming
{
    /// <summary>Пользовательский класс для демонстрации сортировок. Порядок по умолчанию: Age, затем Name.</summary>
    public sealed class Student : IComparable<Student>
    {
        public string Name { get; }
        public int Age { get; }

        public Student(string name, int age)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Age = age;
        }

        public int CompareTo(Student? other)
        {
            if (other is null) return 1;

            int c = Age.CompareTo(other.Age);
            return c != 0 ? c : string.CompareOrdinal(Name, other.Name);
        }

        public override string ToString() => $"{Name} ({Age})";
    }
}
