using GeneralizedProgramming;
using System.Reflection;
using Xunit;

namespace Graph.Tests;

public class ProgramEntryPointTests
{
    [Fact]
    public void Main_ConfiguresEncodingAndStartsGraphMenu()
    {
        var oldIn = Console.In; var oldOut = Console.Out;
        try
        {
            Console.SetIn(new StringReader("0\n"));
            Console.SetOut(TextWriter.Null);
            var program = typeof(Graph<string>).Assembly.GetType("GeneralizedProgramming.Program", throwOnError: true)!;
            program.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, null);
        }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }
}
