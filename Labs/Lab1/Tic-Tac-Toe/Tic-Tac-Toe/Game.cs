using System;
using System.Numerics;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]
namespace TicTacToe
{
    class Game
    {
        private Player player1;
        private Player player2;
        private GameStat gameStat;
        private Player curPlayer;
        private enum GameStat
        {
            Win,
            Draw,
            InProcess
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

        private void MakeMove(int row, int col)
        {
            int target = row * 3 + col;
            if (InputValidator.ValidateMove(target, player1, player2)) curPlayer.map |= (1 << target);
            else
            {
                if ((player1.map & (1 << target)) != 0)
                {
                    Console.WriteLine("This cell is occupied by X!\n");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("This cell is occupied by O!\n");
                    Console.ReadLine();
                }
                SwitchPlayers();
            }
        }
        private void CheckGameStat()
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

        private void SwitchPlayers()
        {
            if (curPlayer == player1) curPlayer = player2;
            else curPlayer = player1;
        }

        public void NewGame()
        {
            player1 = new Player(Player.Values.X);
            player2 = new Player(Player.Values.O);
            gameStat = GameStat.InProcess;
            curPlayer = player1;
        }

        public void Play()
        {
            while (gameStat == GameStat.InProcess)
            {
                Console.WriteLine("\nEnter row: ");
                int row = InputValidator.ReadInt();
                Console.WriteLine("\nEnter column: ");
                int col = InputValidator.ReadInt();
                MakeMove(row, col);
                CheckGameStat();
                SwitchPlayers();
                try { Console.Clear(); }
                catch (IOException) { }
            }
        }
    }
}