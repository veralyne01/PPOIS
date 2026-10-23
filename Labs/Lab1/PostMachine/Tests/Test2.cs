using PostMachine;

namespace PostMachine.Tests { 

[TestClass]
[DoNotParallelize]
public class ProgramTests
{
    private static string Run(params string[] lines)
    {
        var oldIn = Console.In;
        var oldOut = Console.Out;
        var output = new StringWriter();
        try
        {
            Console.SetIn(new StringReader(string.Join("\n", lines) + "\n"));
            Console.SetOut(output);
            Program.Main(new string[0]);
        }
        finally
        {
            Console.SetIn(oldIn);
            Console.SetOut(oldOut);
        }
        return output.ToString();
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void Exit_Immediately_ShowsHeaderAndStartPosition()
    {
        string output = Run("7");
        StringAssert.Contains(output, "POST MACHINE IMITATION");
        StringAssert.Contains(output, "CURRENT POSITION: 0");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void MoveRight_UpdatesDisplayedPosition()
    {
        string output = Run("4", "3", "7");
        StringAssert.Contains(output, "CURRENT POSITION: 3");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void MoveLeft_GoesToNegativePosition()
    {
        string output = Run("3", "2", "7");
        StringAssert.Contains(output, "CURRENT POSITION: -2");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void CheckCell_OnEmptyTape_ReportsNotLabelled()
    {
        string output = Run("6", "", "7");
        StringAssert.Contains(output, "Current cell is not labelled");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void MakeLabel_ThenCheck_ReportsLabelled()
    {
        string output = Run("1", "6", "", "7");
        StringAssert.Contains(output, "Current cell is already labelled");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void RemoveLabel_ThenCheck_ReportsNotLabelled()
    {
        string output = Run("1", "2", "6", "", "7");
        StringAssert.Contains(output, "Current cell is not labelled");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void RemoveLabel_OnEmptyCell_DoesNotCrash()
    {
        string output = Run("2", "6", "", "7");
        StringAssert.Contains(output, "Current cell is not labelled");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void MoveAwayFromLabel_NewCellIsEmpty()
    {
        string output = Run("1", "4", "1", "6", "", "7");
        StringAssert.Contains(output, "Current cell is not labelled");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void MoveBackToLabel_CellIsLabelled()
    {
        string output = Run("1", "4", "2", "3", "2", "6", "", "7");
        StringAssert.Contains(output, "Current cell is already labelled");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void NewTape_ResetsPositionAndLabels()
    {
        string output = Run("1", "4", "5", "5", "6", "", "7");
        StringAssert.Contains(output, "CURRENT POSITION: 5");
        Assert.IsGreaterThan(output.IndexOf("CURRENT POSITION: 5"), output.LastIndexOf("CURRENT POSITION: 0")
);
        StringAssert.Contains(output, "Current cell is not labelled");
    }



    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void InvalidMenuInput_ShowsError_AndProgramContinues()
    {
        string output = Run("abc", "", "7");
        StringAssert.Contains(output, "Invalid data type!");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void UnknownMenuNumber_IsIgnored()
    {
        string output = Run("9", "7");
        StringAssert.Contains(output, "CURRENT POSITION: 0");
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void InvalidStep_ShowsError_PositionUnchanged()
    {
        string output = Run("4", "abc", "", "7");
        StringAssert.Contains(output, "Invalid data type!");
        Assert.DoesNotContain("CURRENT POSITION: 1", output);
    }

    [TestMethod, Timeout(5000, CooperativeCancellation = true)]
    public void NegativeStep_IsRejected_PositionUnchanged()
    {
        string output = Run("3", "-3", "", "7");
        StringAssert.Contains(output, "Invalid data type!");
        Assert.DoesNotContain("CURRENT POSITION: 3", output);
        Assert.DoesNotContain("CURRENT POSITION: -3", output);
    }
}

[TestClass]
public class AdditionAlgorithmTests
{

    private static void WriteNumber(Slider c, int n)
    {
        for (int i = 0; i <= n; i++)
        {
            c.MakeLabel();
            c.MoveRight(1);
        }
    }

    private static Slider PrepareTape(int a, int b)
    {
        var c = new Slider();
        WriteNumber(c, a);
        c.MoveRight(1);
        WriteNumber(c, b);
        c.MoveLeft(c.Position);
        return c;
    }

    private static void RunAddition(Slider c)
    {
        do { c.MoveRight(1); } while (c.CheckCell());
        c.MakeLabel();
        do { c.MoveRight(1); } while (c.CheckCell());
        c.MoveLeft(1);
        c.RemoveLabel();
        c.MoveLeft(1);
        c.RemoveLabel();
    }

    private static int ReadResult(Slider c)
    {
        c.MoveLeft(c.Position);
        int marks = 0;
        while (c.CheckCell())
        {
            marks++;
            c.MoveRight(1);
        }
        return marks - 1;
    }

    [TestMethod]
    [DataRow(0, 0)]
    [DataRow(0, 3)]
    [DataRow(3, 0)]
    [DataRow(1, 1)]
    [DataRow(2, 5)]
    [DataRow(10, 7)]
    public void Addition_ProducesSum(int a, int b)
    {
        var c = PrepareTape(a, b);
        RunAddition(c);
        Assert.AreEqual(a + b, ReadResult(c));
    }

    [TestMethod]
    public void Addition_AfterwardsNoLabelsRemainBeyondResult()
    {
        var c = PrepareTape(2, 2);
        RunAddition(c);
        int sum = ReadResult(c);
        Assert.AreEqual(4, sum);
        Assert.IsFalse(c.CheckCell());
    }
}
}
