namespace Minimarket
{
    public abstract class Employee
    {
        private static int nextEmployeeNumber = 600;

        public int number { get; }
        public string Name { get; }

        protected Employee(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Employee name cannot be empty!", nameof(name));

            number = ++nextEmployeeNumber;
            Name = name;
        }
    }
}
