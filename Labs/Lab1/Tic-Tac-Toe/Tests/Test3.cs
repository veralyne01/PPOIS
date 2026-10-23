using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using TicTacToe;

namespace Tests
{
    [TestClass]
    [DoNotParallelize]
    public class GameTests
    {
        private Game game;
        private TextReader oldIn;
        private TextWriter oldOut;

        private Player Player1 =>
            (Player)GetField("player1");

        private Player Player2 =>
            (Player)GetField("player2");

        private Player CurrentPlayer =>
            (Player)GetField("curPlayer");

        [TestInitialize]
        public void Initialize()
        {
            oldIn = Console.In;
            oldOut = Console.Out;

            Console.SetIn(new StringReader("\n\n\n"));
            Console.SetOut(new StringWriter());

            game = new Game();
            game.NewGame();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetIn(oldIn);
            Console.SetOut(oldOut);
        }

        private object GetField(string name)
        {
            return typeof(Game)
                .GetField(name, BindingFlags.Instance |
                                BindingFlags.NonPublic)
                .GetValue(game);
        }

        private void SetField(string name, object value)
        {
            typeof(Game)
                .GetField(name, BindingFlags.Instance |
                                BindingFlags.NonPublic)
                .SetValue(game, value);
        }

        private void Call(string name, params object[] args)
        {
            typeof(Game)
                .GetMethod(name, BindingFlags.Instance |
                                 BindingFlags.NonPublic)
                .Invoke(game, args);
        }

        private string Output()
        {
            return oldOut is StringWriter
                ? oldOut.ToString()
                : string.Empty;
        }

        [TestMethod]
        public void NewGame_CreatesPlayerX()
        {
            Assert.AreEqual(Player.Values.X, Player1.Sign);
        }

        [TestMethod]
        public void NewGame_CreatesPlayerO()
        {
            Assert.AreEqual(Player.Values.O, Player2.Sign);
        }

        [TestMethod]
        public void NewGame_ClearsPlayerMaps()
        {
            Player1.map = 1 << 0;
            Player2.map = 1 << 1;

            game.NewGame();

            Assert.AreEqual(0, Player1.map);
            Assert.AreEqual(0, Player2.map);
        }

        [TestMethod]
        public void NewGame_SetsCurrentPlayerToX()
        {
            Call("SwitchPlayers");

            game.NewGame();

            Assert.AreSame(Player1, CurrentPlayer);
        }

        [TestMethod]
        public void SwitchPlayers_FromX_SwitchesToO()
        {
            Call("SwitchPlayers");

            Assert.AreSame(Player2, CurrentPlayer);
        }

        [TestMethod]
        public void SwitchPlayers_FromO_SwitchesToX()
        {
            Call("SwitchPlayers");
            Call("SwitchPlayers");

            Assert.AreSame(Player1, CurrentPlayer);
        }

        [TestMethod]
        public void MakeMove_EmptyCell_AddsMarkToCurrentPlayer()
        {
            Call("MakeMove", 0, 0);

            Assert.AreEqual(1 << 0, Player1.map);
            Assert.AreEqual(0, Player2.map);
        }

        [TestMethod]
        public void MakeMove_CenterCell_AddsMarkToCorrectPosition()
        {
            Call("MakeMove", 1, 1);

            Assert.AreEqual(1 << 4, Player1.map);
        }

        [TestMethod]
        public void MakeMove_LastCell_AddsMarkToCorrectPosition()
        {
            Call("MakeMove", 2, 2);

            Assert.AreEqual(1 << 8, Player1.map);
        }

        [TestMethod]
        public void MakeMove_ValidMove_DoesNotChangeCurrentPlayer()
        {
            Call("MakeMove", 0, 0);

            Assert.AreSame(Player1, CurrentPlayer);
        }

        [TestMethod]
        public void MakeMove_CellOccupiedByX_DoesNotOverwriteMark()
        {
            Player1.map = 1 << 0;

            Call("MakeMove", 0, 0);

            Assert.AreEqual(1 << 0, Player1.map);
        }

        [TestMethod]
        public void MakeMove_CellOccupiedByO_DoesNotOverwriteMark()
        {
            Player2.map = 1 << 0;

            Call("MakeMove", 0, 0);

            Assert.AreEqual(0, Player1.map);
            Assert.AreEqual(1 << 0, Player2.map);
        }

        [TestMethod]
        public void MakeMove_OccupiedByX_PrintsCorrectMessage()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);
            Player1.map = 1 << 0;

            Call("MakeMove", 0, 0);

            StringAssert.Contains(
                writer.ToString(),
                "This cell is occupied by X!");
        }

        [TestMethod]
        public void MakeMove_OccupiedByO_PrintsCorrectMessage()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);
            Player2.map = 1 << 0;

            Call("MakeMove", 0, 0);

            StringAssert.Contains(
                writer.ToString(),
                "This cell is occupied by O!");
        }

        [TestMethod]
        public void MakeMove_OccupiedCell_SwitchesCurrentPlayer()
        {
            Player1.map = 1 << 0;

            Call("MakeMove", 0, 0);

            Assert.AreSame(Player2, CurrentPlayer);
        }

        [TestMethod]
        [DataRow(0b000000111)]
        [DataRow(0b000111000)]
        [DataRow(0b111000000)]
        [DataRow(0b001001001)]
        [DataRow(0b100100100)]
        [DataRow(0b001010100)]
        [DataRow(0b100010001)]
        [DataRow(0b010010010)]
        public void CheckGameStat_WinningCombination_SetsWinState(
            int combination)
        {
            Player1.map = combination;

            Call("CheckGameStat");

            Assert.AreEqual("Win", GetField("gameStat").ToString());
        }

        [TestMethod]
        public void CheckGameStat_XWins_PrintsXWinsMessage()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);
            Player1.map = 0b000000111;

            Call("CheckGameStat");

            StringAssert.Contains(writer.ToString(), "Player X wins!");
        }

        [TestMethod]
        public void CheckGameStat_OWins_PrintsOWinsMessage()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);
            Player2.map = 0b000000111;
            Call("SwitchPlayers");

            Call("CheckGameStat");

            StringAssert.Contains(writer.ToString(), "Player O wins!");
        }

        [TestMethod]
        public void CheckGameStat_NoWinningCombination_KeepsGameInProcess()
        {
            Player1.map = (1 << 0) | (1 << 4);
            Player2.map = (1 << 1) | (1 << 5);

            Call("CheckGameStat");

            Assert.AreEqual(
                "InProcess",
                GetField("gameStat").ToString());
        }

        [TestMethod]
        public void CheckGameStat_DrawBoard_SetsDrawState()
        {
            Player1.map =
                (1 << 0) | (1 << 2) | (1 << 3) |
                (1 << 7) | (1 << 8);

            Player2.map =
                (1 << 1) | (1 << 4) | (1 << 5) | (1 << 6);

            Call("CheckGameStat");

            Assert.AreEqual(
                "Draw",
                GetField("gameStat").ToString());
        }

        [TestMethod]
        public void CheckGameStat_DrawBoard_PrintsDrawMessage()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            Player1.map =
                (1 << 0) | (1 << 2) | (1 << 3) |
                (1 << 7) | (1 << 8);

            Player2.map =
                (1 << 1) | (1 << 4) | (1 << 5) | (1 << 6);

            Call("CheckGameStat");

            StringAssert.Contains(
                writer.ToString(),
                "Friendship wins");
        }

        [TestMethod]
        public void NewGame_AfterMoves_ResetsGameState()
        {
            Player1.map = 1 << 0;
            Player2.map = 1 << 1;
            Call("SwitchPlayers");

            game.NewGame();

            Assert.AreEqual(0, Player1.map);
            Assert.AreEqual(0, Player2.map);
            Assert.AreSame(Player1, CurrentPlayer);
            Assert.AreEqual(
                "InProcess",
                GetField("gameStat").ToString());
        }
        private string RunPlay(params string[] inputLines)
        {
            Console.SetIn(new StringReader(string.Join("\n", inputLines) + "\n"));
            var writer = new StringWriter();
            Console.SetOut(writer);

            game.NewGame();
            game.Play();

            return writer.ToString();
        }

        [TestMethod]
        public void Play_XWinsByTopRow_PrintsXWins()
        {
            // X(0,0) O(1,0) X(0,1) O(1,1) X(0,2), потом Enter после победы
            string output = RunPlay(
                "0", "0",
                "1", "0",
                "0", "1",
                "1", "1",
                "0", "2",
                "");

            StringAssert.Contains(output, "Player X wins!");
            Assert.AreEqual("Win", GetField("gameStat").ToString());
        }

        [TestMethod]
        public void Play_OWinsByMiddleRow_PrintsOWins()
        {
            // X(0,0) O(1,0) X(0,1) O(1,1) X(2,2) O(1,2)
            string output = RunPlay(
                "0", "0",
                "1", "0",
                "0", "1",
                "1", "1",
                "2", "2",
                "1", "2",
                "");

            StringAssert.Contains(output, "Player O wins!");
            Assert.AreEqual("Win", GetField("gameStat").ToString());
        }

        [TestMethod]
        public void Play_FullBoardWithoutWinner_PrintsDraw()
        {
            // Порядок клеток: X0 O1 X2 O4 X3 O5 X7 O6 X8
            string output = RunPlay(
                "0", "0",
                "0", "1",
                "0", "2",
                "1", "1",
                "1", "0",
                "1", "2",
                "2", "1",
                "2", "0",
                "2", "2",
                "");

            StringAssert.Contains(output, "Friendship wins");
            Assert.AreEqual("Draw", GetField("gameStat").ToString());
        }

        [TestMethod]
        public void Play_MoveToOccupiedCell_PrintsMessageAndGameContinues()
        {
            // X(0,0), O(0,0) занято, Enter, затем O ходит повторно
            // (из-за двойного SwitchPlayers), в итоге X добирает верхнюю строку
            string output = RunPlay(
                "0", "0",
                "0", "0",
                "",
                "1", "0",
                "0", "1",
                "1", "1",
                "0", "2",
                "");

            StringAssert.Contains(output, "This cell is occupied by X!");
            StringAssert.Contains(output, "Player X wins!");
        }
    }
}