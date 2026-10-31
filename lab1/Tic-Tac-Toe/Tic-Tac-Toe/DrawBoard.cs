using System;

namespace TicTacToe
{
    /// <summary>
    /// Отрисовка игрового поля 3x3 в консоли по битовым маскам игроков.
    /// </summary>
    internal class DrawBoard
    {
        /// <summary>
        /// Выводит в консоль текущее состояние поля.
        /// Бит с номером <c>row * 3 + col</c> в маске соответствует клетке (row, col).
        /// </summary>
        /// <param name="map1">Маска клеток первого игрока (X).</param>
        /// <param name="map2">Маска клеток второго игрока (O).</param>
        public static void Draw(int map1, int map2)
        {
            Console.WriteLine($"\n{GetSymbol(map1, map2, 0, 0)} | {GetSymbol(map1, map2, 0, 1)} | {GetSymbol(map1, map2, 0, 2)}");
            Console.WriteLine($"\n------------");
            Console.WriteLine($"\n{GetSymbol(map1, map2, 1, 0)} | {GetSymbol(map1, map2, 1, 1)} | {GetSymbol(map1, map2, 1, 2)}");
            Console.WriteLine($"\n------------");
            Console.WriteLine($"\n{GetSymbol(map1, map2, 2, 0)} | {GetSymbol(map1, map2, 2, 1)} | {GetSymbol(map1, map2, 2, 2)}\n");
        }

        /// <summary>
        /// Определяет символ, который нужно нарисовать в клетке.
        /// </summary>
        /// <param name="map1">Маска клеток первого игрока (X).</param>
        /// <param name="map2">Маска клеток второго игрока (O).</param>
        /// <param name="row">Номер строки (от 0 до 2).</param>
        /// <param name="col">Номер столбца (от 0 до 2).</param>
        /// <returns><c>'X'</c>, <c>'O'</c> или пробел, если клетка пуста.</returns>
        private static char GetSymbol(int map1, int map2, int row, int col)
        {
            int bit = 1 << (row * 3 + col);
            if ((map1 & bit) != 0) return 'X';
            else if ((map2 & bit) != 0) return 'O';
            return ' ';
        }
    }
}