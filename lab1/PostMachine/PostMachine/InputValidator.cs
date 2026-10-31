using System;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("PostMachine_Tests")]

namespace PostMachine
{
    /// <summary>
    /// Проверка пользовательского ввода для консольного меню машины Поста.
    /// </summary>
    public class InputValidator
    {
        /// <summary>
        /// Читает строку из консоли и преобразует её в неотрицательное целое число.
        /// </summary>
        /// <returns>Введённое неотрицательное число.</returns>
        /// <exception cref="FormatException">
        /// Строка не является целым числом, число отрицательное
        /// или ввод закончился (<c>Console.ReadLine()</c> вернул <c>null</c>).
        /// </exception>
        public static int ValidateInt()
        {
            if (int.TryParse(Console.ReadLine(), out int input) && input >= 0) return input;
            else throw new FormatException("Invalid data type!\n");
        }
    }
}