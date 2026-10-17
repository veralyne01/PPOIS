using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Numerics;
using TicTacToe;

namespace Tests
{
    [DoNotParallelize]
    [TestClass]
    public class InputValidatorTests
    {
        private TextReader oldIn;

        [TestInitialize]
        public void Initialize()
        {
            oldIn = Console.In;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetIn(oldIn);
        }

        [TestMethod]
        [DataRow("0", 0)]
        [DataRow("1", 1)]
        [DataRow("2", 2)]
        public void ValidateInt_ValidInput_ReturnsValue(
            string input, int expected)
        {
            Console.SetIn(new StringReader(input));

            Assert.AreEqual(expected, InputValidator.ValidateInt());
        }

        [TestMethod]
        [DataRow("-1")]
        [DataRow("3")]
        [DataRow("100")]
        public void ValidateInt_OutOfRange_ThrowsException(string input)
        {
            Console.SetIn(new StringReader(input));

            Assert.Throws<Exception>(
                () => InputValidator.ValidateInt());
        }

        [TestMethod]
        [DataRow("abc")]
        [DataRow("1.5")]
        [DataRow("")]
        public void ValidateInt_InvalidFormat_ThrowsFormatException(
            string input)
        {
            Console.SetIn(new StringReader(input));

            Assert.Throws<Exception>(
                () => InputValidator.ValidateInt());
        }

        [TestMethod]
        public void ValidateMove_EmptyCell_ReturnsTrue()
        {
            var player1 = new Player(Player.Values.X);
            var player2 = new Player(Player.Values.O);

            Assert.IsTrue(
                InputValidator.ValidateMove(0, player1, player2));
        }

        [TestMethod]
        public void ValidateMove_CellOccupiedByX_ReturnsFalse()
        {
            var player1 = new Player(Player.Values.X);
            var player2 = new Player(Player.Values.O);
            player1.map |= 1 << 4;

            Assert.IsFalse(
                InputValidator.ValidateMove(4, player1, player2));
        }

        [TestMethod]
        public void ValidateMove_CellOccupiedByO_ReturnsFalse()
        {
            var player1 = new Player(Player.Values.X);
            var player2 = new Player(Player.Values.O);
            player2.map |= 1 << 4;

            Assert.IsFalse(
                InputValidator.ValidateMove(4, player1, player2));
        }

        [TestMethod]
        public void ValidateMove_DoesNotChangePlayerMaps()
        {
            var player1 = new Player(Player.Values.X);
            var player2 = new Player(Player.Values.O);
            player1.map = 1 << 0;
            player2.map = 1 << 4;

            InputValidator.ValidateMove(8, player1, player2);

            Assert.AreEqual(1 << 0, player1.map);
            Assert.AreEqual(1 << 4, player2.map);
        }

        [TestMethod]
        public void ReadInt_InvalidThenValidInput_ReturnsValidValue()
        {
            var oldIn = Console.In;
            var oldOut = Console.Out;

            try
            {
                Console.SetIn(new StringReader("abc\n3\n2\n"));
                Console.SetOut(new StringWriter());

                Assert.AreEqual(2, InputValidator.ReadInt());
            }
            finally
            {
                Console.SetIn(oldIn);
                Console.SetOut(oldOut);
            }
        }
    }
}