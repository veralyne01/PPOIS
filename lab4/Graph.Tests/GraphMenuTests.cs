using System.Reflection;
using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphMenuTests
{
    private static void Execute(GraphMenu menu,string choice)
        => typeof(GraphMenu).GetMethod("Execute",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(menu,new object[]{choice});

    private static string RunWithInput(Action action,string input)
    {
        var oldIn=Console.In; var oldOut=Console.Out; var output=new StringWriter();
        try { Console.SetIn(new StringReader(input)); Console.SetOut(output); action(); return output.ToString(); }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }

    [Fact]
    public void Execute_CoversAllMenuCommandsAndBothTraversalDirections()
    {
        var menu=new GraphMenu();
        RunWithInput(()=>Execute(menu,"14"),"");
        RunWithInput(()=>Execute(menu,"15"),"");
        RunWithInput(()=>Execute(menu,"17"),"");
        RunWithInput(()=>Execute(menu,"1"),"Extra\n");
        RunWithInput(()=>Execute(menu,"2"),"4\n0\n");
        RunWithInput(()=>Execute(menu,"3"),"4\n");
        RunWithInput(()=>Execute(menu,"4"),"0\n1\n");
        RunWithInput(()=>Execute(menu,"5"),"0\n");
        RunWithInput(()=>Execute(menu,"6"),"0\n2\n");
        RunWithInput(()=>Execute(menu,"7"),"");
        RunWithInput(()=>Execute(menu,"8"),"0\n");
        RunWithInput(()=>Execute(menu,"9"),"0\n2\n");
        RunWithInput(()=>Execute(menu,"10"),"1\n");
        RunWithInput(()=>Execute(menu,"10"),"2\n");
        RunWithInput(()=>Execute(menu,"11"),"1\n");
        RunWithInput(()=>Execute(menu,"11"),"2\n");
        RunWithInput(()=>Execute(menu,"12"),"0\n1\n");
        RunWithInput(()=>Execute(menu,"12"),"0\n2\n");
        RunWithInput(()=>Execute(menu,"13"),"0\n1\n");
        RunWithInput(()=>Execute(menu,"13"),"0\n2\n");
        RunWithInput(()=>Execute(menu,"14"),"1\n");
        RunWithInput(()=>Execute(menu,"15"),"1\n");
        RunWithInput(()=>Execute(menu,"16"),"");
        RunWithInput(()=>Execute(menu,"18"),"");
        RunWithInput(()=>Execute(menu,"19"),"");
        RunWithInput(()=>Execute(menu,"1"),"Solo\n");
        RunWithInput(()=>Execute(menu,"2"),"0\n0\n");
        RunWithInput(()=>Execute(menu,"14"),"1\n");
        RunWithInput(()=>Execute(menu,"15"),"");
        var output=RunWithInput(()=>Execute(menu,"unknown"),"");
        Assert.Contains("Error.",output);
    }

    [Fact]
    public void ReadInt_RepeatsAfterInvalidInput()
    {
        var menu=new GraphMenu();
        var output=RunWithInput(()=>Execute(menu,"5"),"oops\n0\n");
        Assert.Contains("Enter number.",output);
        Assert.Contains("Vertex not found.",output);
    }

    [Fact]
    public void ReadReverse_RepeatsAfterInvalidChoice()
    {
        var menu=new GraphMenu();
        var output=RunWithInput(()=>Execute(menu,"10"),"bad\n3\n1\n");
        Assert.Contains("Enter 1 or 2.",output);
    }

    [Fact]
    public void IsolatedVertexTraversalPrintsDashForEmptyIncidentAndAdjacentCollections()
    {
        var menu=new GraphMenu();
        RunWithInput(()=>Execute(menu,"1"),"Solo\n");
        var incident=RunWithInput(()=>Execute(menu,"12"),"0\n1\n");
        Assert.Contains("Forward (MoveNext): -",incident);
        var adjacent=RunWithInput(()=>Execute(menu,"13"),"0\n2\n");
        Assert.Contains("Forward (MoveNext): -",adjacent);
    }

    [Fact]
    public void RemoveVertexViaIterator_RejectsOutOfRangeNumber()
    {
        var menu=new GraphMenu();
        RunWithInput(()=>Execute(menu,"17"),"");
        var output=RunWithInput(()=>Execute(menu,"14"),"0\n");
        Assert.Contains("Number is out of range.",output);
    }

    [Fact]
    public void Run_CatchesGraphExceptionsAndContinuesUntilExit()
    {
        var menu=new GraphMenu();
        var output=RunWithInput(()=>menu.Run(),"4\n10\n11\n18\n0\n");
        Assert.Contains("Error:",output);
        Assert.Contains("Directed graph", output);
    }

    [Fact]
    public void Run_HandlesUnknownChoice()
    {
        var menu=new GraphMenu();
        var output=RunWithInput(()=>menu.Run(),"abc\n0\n");
        Assert.Contains("Error.",output);
    }

    [Fact]
    public void Run_ReadsTrimmedExitChoice()
    {
        var menu=new GraphMenu();
        RunWithInput(()=>menu.Run()," 0 \n");
    }
}
