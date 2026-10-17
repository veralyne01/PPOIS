using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostMachine;

namespace PostMachine.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class TapeTests
    {
        [TestMethod]
        public void MakeLabel_LabelUnderSlider()
        {
            var slider = new Slider();
            slider.MakeLabel();
            Assert.IsTrue(slider.CheckCell());
        }

        [TestMethod]
        public void CheckUnmarkedCell_ReturnFalse()
        {
            var slider = new Slider();
            Assert.IsFalse(slider.CheckCell());
        }

        [TestMethod]
        public void MakeLabelAfterChecking()
        {
            var slider = new Slider();
            Assert.IsFalse(slider.CheckCell());
            slider.MakeLabel();
            Assert.IsTrue(slider.CheckCell());
        }

        [TestMethod]
        public void RemoveNewLabel()
        {
            var slider = new Slider();
            slider.MakeLabel();
            Assert.IsTrue(slider.CheckCell());
            slider.RemoveLabel();
            Assert.IsFalse(slider.CheckCell());
        }

        [TestMethod]
        public void NewTape_CheckPos_ReturnZero()
        {
            var slider = new Slider();
            slider.MoveLeft(5);
            slider.Reset();
            Assert.AreEqual(0, slider.Position);
        }

        [TestMethod]
        public void NewTape_MoveRight_CheckIsLabelled_ReturnFalse()
        {
            var slider = new Slider();
            slider.MoveRight(3);
            slider.MakeLabel();
            slider.Reset();
            slider.MoveRight(3);
            Assert.IsFalse(slider.CheckCell());
        }

        [TestMethod]
        public void NewTape_MoveLeft_CheckIsLabelled_ReturnFalse()
        {
            var slider = new Slider();
            slider.MoveLeft(3);
            slider.MakeLabel();
            slider.Reset();
            slider.MoveLeft(3);
            Assert.IsFalse(slider.CheckCell());
        }

        [TestMethod]
        public void NewTape_CheckIsLabelled_ReturnFalse()
        {
            var slider = new Slider();
            slider.MakeLabel();
            slider.Reset();
            Assert.IsFalse(slider.CheckCell());
        }
    }

    [TestClass]
    [DoNotParallelize]
    public class SliderTests
    {

        [TestMethod]
        public void NewSlider_PositionIsZero()
        {
            var slider = new Slider();
            Assert.AreEqual(0, slider.Position);
        }

        [TestMethod]
        public void MoveRight_PositiveStep_IncreasesPosition()
        {
            var slider = new Slider();
            slider.MoveRight(3);
            Assert.AreEqual(3, slider.Position);
        }

        [TestMethod]
        public void MoveLeft_PositiveStep_DecreasesPosition()
        {
            var slider = new Slider();
            slider.MoveLeft(3);
            Assert.AreEqual(-3, slider.Position);
        }

        [TestMethod]
        public void MoveLeft_FromZero_GoesToNegativePosition()
        {
            var slider = new Slider();
            slider.MoveLeft(1);
            Assert.IsLessThan(0, slider.Position);
        }

        [TestMethod]
        public void MoveRightThenLeft_SameStep_ReturnsToStart()
        {
            var slider = new Slider();
            slider.MoveRight(5);
            slider.MoveLeft(5);
            Assert.AreEqual(0, slider.Position);
        }

        [TestMethod]
        public void MoveRight_SeveralTimes_StepsAccumulate()
        {
            var slider = new Slider();
            slider.MoveRight(2);
            slider.MoveRight(4);
            Assert.AreEqual(6, slider.Position);
        }

        [TestMethod]
        public void Move_ZeroStep_PositionUnchanged()
        {
            var slider = new Slider();
            slider.MoveRight(0);
            slider.MoveLeft(0);
            Assert.AreEqual(0, slider.Position);
        }

        [TestMethod]
        public void MoveRight_LargeStep_PositionEqualsStep()
        {
            var slider = new Slider();
            slider.MoveRight(1000000);
            Assert.AreEqual(1000000, slider.Position);
        }

        [TestMethod]
        public void MoveLeft_LargeStep_PositionIsNegativeStep()
        {
            var slider = new Slider();
            slider.MoveLeft(1000000);
            Assert.AreEqual(-1000000, slider.Position);
        }

        [TestMethod]
        public void MoveRight_NegativeStep_MovesLeft()
        {
            var slider = new Slider();
            slider.MoveRight(-4);
            Assert.AreEqual(-4, slider.Position);
        }

        [TestMethod]
        public void MoveLeft_NegativeStep_MovesRight()
        {
            var slider = new Slider();
            slider.MoveLeft(-4);
            Assert.AreEqual(4, slider.Position);
        }

        [TestMethod]
        public void MoveAfterMakeLabel_NewCellIsEmpty()
        {
            var slider = new Slider();
            slider.MakeLabel();
            slider.MoveRight(1);
            Assert.IsFalse(slider.CheckCell());
        }

        [TestMethod]
        public void MoveBackToLabel_CellIsStillMarked()
        {
            var slider = new Slider();
            slider.MakeLabel();
            slider.MoveRight(2);
            slider.MoveLeft(2);
            Assert.IsTrue(slider.CheckCell());
        }
    }

    [TestClass]
    [DoNotParallelize]
    public class InputValidatorTests
    {
        private static void SetInput(string text)
        {
            Console.SetIn(new StringReader(text));
        }

        [TestMethod]
        public void ValidateInt_ValidNumber_ReturnsNumber()
        {
            SetInput("42");
            Assert.AreEqual(42, InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateInt_Zero_IsAccepted()
        {
            SetInput("0");
            Assert.AreEqual(0, InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateInt_MaxInt_IsAccepted()
        {
            SetInput(int.MaxValue.ToString());
            Assert.AreEqual(int.MaxValue, InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateInt_NegativeNumber_ThrowsFormatException()
        {
            SetInput("-5");
            Assert.Throws<FormatException>(() => InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateInt_Letters_ThrowsFormatException()
        {
            SetInput("abc");
            Assert.Throws<FormatException>(() => InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateInt_EmptyLine_ThrowsFormatException()
        {
            SetInput("\n");
            Assert.Throws<FormatException>(() => InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateIntEndOfInput_ThrowsFormatException()
        {
            SetInput("");
            Assert.Throws<FormatException>(() => InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateInt_FractionalNumber_ThrowsFormatException()
        {
            SetInput("1.5");
            Assert.Throws<FormatException>(() => InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateInt_NumberTooLargeForInt_ThrowsFormatException()
        {
            SetInput("99999999999");
            Assert.Throws<FormatException>(() => InputValidator.ValidateInt());
        }
    }

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

        [TestMethod, Timeout(5000)]
        public void Exit_Immediately_ShowsHeaderAndStartPosition()
        {
            string output = Run("7");
            StringAssert.Contains(output, "POST MACHINE IMITATION");
            StringAssert.Contains(output, "CURRENT POSITION: 0");
        }

        [TestMethod, Timeout(5000)]
        public void MoveRight_UpdatesDisplayedPosition()
        {
            string output = Run("4", "3", "7");
            StringAssert.Contains(output, "CURRENT POSITION: 3");
        }

        [TestMethod, Timeout(5000)]
        public void MoveLeft_GoesToNegativePosition()
        {
            string output = Run("3", "2", "7");
            StringAssert.Contains(output, "CURRENT POSITION: -2");
        }

        [TestMethod, Timeout(5000)]
        public void CheckCell_OnEmptyTape_ReportsNotLabelled()
        {
            string output = Run("6", "", "7");
            StringAssert.Contains(output, "Current cell is not labelled");
        }

        [TestMethod, Timeout(5000)]
        public void MakeLabel_ThenCheck_ReportsLabelled()
        {
            string output = Run("1", "6", "", "7");
            StringAssert.Contains(output, "Current cell is already labelled");
        }

        [TestMethod, Timeout(5000)]
        public void RemoveLabel_ThenCheck_ReportsNotLabelled()
        {
            string output = Run("1", "2", "6", "", "7");
            StringAssert.Contains(output, "Current cell is not labelled");
        }

        [TestMethod, Timeout(5000)]
        public void RemoveLabel_OnEmptyCell_DoesNotCrash()
        {
            string output = Run("2", "6", "", "7");
            StringAssert.Contains(output, "Current cell is not labelled");
        }

        [TestMethod, Timeout(5000)]
        public void MoveAwayFromLabel_NewCellIsEmpty()
        {
            string output = Run("1", "4", "1", "6", "", "7");
            StringAssert.Contains(output, "Current cell is not labelled");
        }

        [TestMethod, Timeout(5000)]
        public void MoveBackToLabel_CellIsLabelled()
        {
            string output = Run("1", "4", "2", "3", "2", "6", "", "7");
            StringAssert.Contains(output, "Current cell is already labelled");
        }

        [TestMethod, Timeout(5000)]
        public void NewTape_ResetsPositionAndLabels()
        {
            string output = Run("1", "4", "5", "5", "6", "", "7");
            StringAssert.Contains(output, "CURRENT POSITION: 5");
            Assert.IsTrue(output.LastIndexOf("CURRENT POSITION: 0")
                          > output.IndexOf("CURRENT POSITION: 5"));
            StringAssert.Contains(output, "Current cell is not labelled");
        }



        [TestMethod, Timeout(5000)]
        public void InvalidMenuInput_ShowsError_AndProgramContinues()
        {
            string output = Run("abc", "", "7");
            StringAssert.Contains(output, "Invalid data type!");
        }

        [TestMethod, Timeout(5000)]
        public void UnknownMenuNumber_IsIgnored()
        {
            string output = Run("9", "7");
            StringAssert.Contains(output, "CURRENT POSITION: 0");
        }

        [TestMethod, Timeout(5000)]
        public void InvalidStep_ShowsError_PositionUnchanged()
        {
            string output = Run("4", "abc", "", "7");
            StringAssert.Contains(output, "Invalid data type!");
            Assert.IsFalse(output.Contains("CURRENT POSITION: 1"));
        }

        [TestMethod, Timeout(5000)]
        public void NegativeStep_IsRejected_PositionUnchanged()
        {
            string output = Run("3", "-3", "", "7");
            StringAssert.Contains(output, "Invalid data type!");
            Assert.IsFalse(output.Contains("CURRENT POSITION: 3"));
            Assert.IsFalse(output.Contains("CURRENT POSITION: -3"));
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