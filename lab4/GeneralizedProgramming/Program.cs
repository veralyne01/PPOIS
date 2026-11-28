using System.Text;

namespace GeneralizedProgramming
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var sortingMenu = new SortingMenu();
            sortingMenu.Run();
        }
    }
}
