namespace EcologyProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Ecosystem ecosystem = new();
            MenuUI.Menu(ecosystem);
        }
    }
}