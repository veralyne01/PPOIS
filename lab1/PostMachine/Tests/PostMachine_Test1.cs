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
}