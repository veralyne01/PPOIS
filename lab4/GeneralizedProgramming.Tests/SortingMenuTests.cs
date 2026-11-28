using System.Reflection;
using Xunit;

namespace GeneralizedProgramming.Tests;

public class SortingMenuTests
{
    private static object? Invoke(object target, string name, params object?[] args)
        => target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, args);

    private static object? InvokeStatic(string name, params object?[] args)
        => typeof(SortingMenu).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, args);

    [Fact] public void ParseInts_ParsesSupportedSeparators() { var x = (List<int>)InvokeStatic("ParseInts", "1, 2;3\t-4")!; Assert.Equal(new[] { 1, 2, 3, -4 }, x); }
    [Fact] public void ParseInts_EmptyInputReturnsEmptyList() => Assert.Empty((List<int>)InvokeStatic("ParseInts", " , ; ")!);
    [Fact] public void ParseInts_InvalidTokenThrowsFormatException() { var e = Assert.Throws<TargetInvocationException>(() => InvokeStatic("ParseInts", "1 nope")); Assert.IsType<FormatException>(e.InnerException); }
    [Fact] public void ParseStrings_ParsesTokens() { var x = (List<string>)InvokeStatic("ParseStrings", "red, blue;green\tyellow")!; Assert.Equal(new[] { "red", "blue", "green", "yellow" }, x); }
    [Fact] public void ParseStrings_EmptyInputReturnsEmptyList() => Assert.Empty((List<string>)InvokeStatic("ParseStrings", " ")!);
    [Fact] public void ParseStudents_ParsesLastColonAsSeparator() { var x = (List<Student>)InvokeStatic("ParseStudents", "Mary:Jane:21 Bob:19")!; Assert.Equal("Mary:Jane", x[0].Name); Assert.Equal(21, x[0].Age); }
    [Theory]
    [InlineData("Anna")]
    [InlineData(":20")]
    [InlineData("Anna:")]
    public void ParseStudents_InvalidShapeThrowsFormatException(string input) { var e = Assert.Throws<TargetInvocationException>(() => InvokeStatic("ParseStudents", input)); Assert.IsType<FormatException>(e.InnerException); }
    [Fact] public void ParseStudents_InvalidAgeThrowsFormatException() { var e = Assert.Throws<TargetInvocationException>(() => InvokeStatic("ParseStudents", "Anna:abc")); Assert.IsType<FormatException>(e.InnerException); }

    [Fact]
    public void ReadCount_RepeatsUntilValidValue()
    {
        var oldIn = Console.In; var oldOut = Console.Out; var output = new StringWriter();
        try { Console.SetIn(new StringReader("-1\n1001\nabc\n0\n")); Console.SetOut(output); var value = (int)InvokeStatic("ReadCount")!; Assert.Equal(0, value); Assert.Contains("Enter an integer from 0 to 1000.", output.ToString()); }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }

    [Fact]
    public void ReadChoice_RepeatsUntilInRange()
    {
        var oldIn = Console.In; var oldOut = Console.Out; var output = new StringWriter();
        try { Console.SetIn(new StringReader("x\n0\n3\n2\n")); Console.SetOut(output); Assert.Equal(2, (int)InvokeStatic("ReadChoice", "Type", "1 or 2", 1, 2)!); Assert.Contains("Enter a number from 1 to 2.", output.ToString()); }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }

    [Fact]
    public void Run_ReportsFormatErrorAndContinues()
    {
        var oldIn = Console.In;
        var oldOut = Console.Out;
        var output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader("1\n1\n1\n1\nnot-an-int\n0\n"));
            Console.SetOut(output);

            new SortingMenu().Run();

            Assert.Contains("Input error:", output.ToString());
        }
        finally
        {
            Console.SetIn(oldIn);
            Console.SetOut(oldOut);
        }
    }

    [Fact]
    public void Run_HandlesEmptyManualInput()
    {
        var oldIn = Console.In;
        var oldOut = Console.Out;
        var output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader("2\n2\n2\n1\n   \n0\n"));
            Console.SetOut(output);

            new SortingMenu().Run();

            Assert.Contains("(empty)", output.ToString());
        }
        finally
        {
            Console.SetIn(oldIn);
            Console.SetOut(oldOut);
        }
    }

    [Fact]
    public void Run_ExitsImmediatelyOnZero()
    {
        var oldIn = Console.In; var oldOut = Console.Out;
        try { Console.SetIn(new StringReader("0\n")); Console.SetOut(TextWriter.Null); new SortingMenu().Run(); }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }

    [Fact]
    public void Run_ReportsUnknownMenuItemAndThenExits()
    {
        var oldIn = Console.In; var oldOut = Console.Out; var output = new StringWriter();
        try { Console.SetIn(new StringReader("bad\n0\n")); Console.SetOut(output); new SortingMenu().Run(); Assert.Contains("Unknown menu item.", output.ToString()); }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }

    [Theory]
    [InlineData("1\n1\n1\n1\n3 1 2\n0\n")]
    [InlineData("2\n1\n1\n1\n3 1 2\n0\n")]
    [InlineData("1\n2\n2\n1\npear apple\n0\n")]
    [InlineData("2\n2\n2\n1\npear apple\n0\n")]
    [InlineData("1\n3\n1\n1\nAnna:20 Bob:18\n0\n")]
    [InlineData("2\n3\n1\n1\nAnna:20 Bob:18\n1\n0\n")]
    [InlineData("2\n3\n1\n1\nAnna:20 Bob:18\n2\n0\n")]
    public void Run_ExecutesManualScenarios(string input)
    {
        var oldIn = Console.In; var oldOut = Console.Out;
        try { Console.SetIn(new StringReader(input)); Console.SetOut(TextWriter.Null); new SortingMenu().Run(); }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }

    [Theory]
    [InlineData("1\n1\n2\n2\n5\n0\n")]
    [InlineData("2\n2\n2\n2\n5\n0\n")]
    [InlineData("2\n3\n2\n2\n5\n1\n0\n")]
    public void Run_ExecutesRandomScenarios(string input)
    {
        var oldIn = Console.In; var oldOut = Console.Out;
        try { Console.SetIn(new StringReader(input)); Console.SetOut(TextWriter.Null); new SortingMenu().Run(); }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }
}
