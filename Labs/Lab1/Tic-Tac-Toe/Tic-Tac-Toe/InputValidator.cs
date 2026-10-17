using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Tests")]

namespace TicTacToe
{
    class InputValidator
    {
        public static int ValidateInt()
        {
            if (int.TryParse(Console.ReadLine(), out int input) && input >= 0 && input <= 2) return input;
            else if (input < 0 || input > 2) throw new Exception("Index is out of range!\n");
            else throw new FormatException("Invalid data type!\n");
        }

        public static bool ValidateMove(int target, Player player1, Player player2)
        {
            if (((player1.map | player2.map) & (1 << target)) == 0) return true;
            else return false;
        }

        public static int ReadInt()
        {
            int? res = null;
            while (res == null)
            {
                try { res = ValidateInt(); }
                catch (Exception e) { Console.WriteLine(e); }
            }
            return (int)res;
        }
    }
}