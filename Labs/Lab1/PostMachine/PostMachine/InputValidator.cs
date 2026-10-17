using System;

namespace PostMachine
{
    public class InputValidator
    {
        public static int ValidateInt()
        {
            if (int.TryParse(Console.ReadLine(), out int input) && input >= 0) return input;
            else throw new FormatException("Invalid data type!\n");
        }
    }
}