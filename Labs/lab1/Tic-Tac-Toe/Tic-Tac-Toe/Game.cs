using System;
using System.Numerics;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]
namespace TicTacToe
{
    class Game
    {
        protected Player player1;
        protected Player player2;
        protected GameStat gameStat;
        protected Player curPlayer;

        public Game()
        {
            player1 = new Player(Values.X);
            player2 = new Player(Values.O);
            gameStat = GameStat.InProcess;
            curPlayer = player1;
        }
        private int[] winCombinations =
        {
            0b000000111,
            0b000111000,
            0b111000000,
            0b001001001,
            0b100100100,
            0b001010100,
            0b100010001,
            0b010010010
        };

        protected void CheckGameStat()
        {
            if (winCombinations.Any(comb => (comb & curPlayer.map) == comb))
            {
                gameStat = GameStat.Win;
                Console.WriteLine($"Player {curPlayer.Sign} wins!");
                Console.ReadLine();
            }
            else if ((player1.map | player2.map) == 0b111111111)
            {
                gameStat = GameStat.Draw;
                Console.WriteLine("Friendship wins");
                Console.ReadLine();
            }
        }

        protected void SwitchPlayers()
        {
            if (curPlayer == player1) curPlayer = player2;
            else curPlayer = player1;
        }
    }
}