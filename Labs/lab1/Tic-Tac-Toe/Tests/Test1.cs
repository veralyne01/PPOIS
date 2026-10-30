using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using TicTacToe;

namespace Tests
{
    [TestClass]
    [DoNotParallelize]
    public class PlayerTests
    {
        [TestMethod]
        public void Constructor_X_SetsSignAndEmptyMap()
        {
            var player = new Player(Values.X);

            Assert.AreEqual(Values.X, player.Sign);
            Assert.AreEqual(0, player.map);
        }

        [TestMethod]
        public void Constructor_O_SetsSignAndEmptyMap()
        {
            var player = new Player(Values.O);

            Assert.AreEqual(Values.O, player.Sign);
            Assert.AreEqual(0, player.map);
        }

        [TestMethod]
        public void Players_HaveIndependentMaps()
        {
            var player1 = new Player(Values.X);
            var player2 = new Player(Values.O);

            player1.map |= 1 << 0;

            Assert.AreEqual(1, player1.map);
            Assert.AreEqual(0, player2.map);
        }
    }
}