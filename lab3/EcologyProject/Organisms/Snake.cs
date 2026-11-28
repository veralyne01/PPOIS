namespace EcologyProject;

public class Snake : Reptile
{
    public Snake(string name = "Snake", IEnumerable<string>? food = null)
        : base(name, food ?? new[] { "Mouse", "Frog", "Insect" }) { }
}
