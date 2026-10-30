using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    class Gameplay : Game
    {
        private bool MakeMove(int row, int col)
        {
            int target = row * 3 + col;
            if (InputValidator.ValidateMove(target, player1.map, player2.map)) curPlayer.map |= (1 << target);
            else
            {
                if ((player1.map & (1 << target)) != 0)
                {
                    Console.WriteLine("This cell is occupied by X!\n");
                }
                else
                {
                    Console.WriteLine("This cell is occupied by O!\n");
                }
                SwitchPlayers();
                return false;
            }
            return true;
        }

        public void Play()
        {
            while (gameStat == GameStat.InProcess)
            {
                Console.WriteLine("Enter row: ");
                int row = InputValidator.ReadInt();
                Console.WriteLine("\nEnter column: ");
                int col = InputValidator.ReadInt();
                if (MakeMove(row, col)) Console.WriteLine($"\nPlayer {curPlayer.Sign} moves on row {row} column {col}");
                CheckGameStat();
                Console.ReadLine();
                SwitchPlayers();
                try { Console.Clear(); }
                catch (IOException) { }
            }
        }
    }
}
