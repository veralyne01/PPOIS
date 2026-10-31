using System;
using System.IO;

namespace TicTacToe
{
    /// <summary>
    /// Игровой процесс консольных крестиков-ноликов: ввод ходов,
    /// отрисовка поля и смена игроков до завершения партии.
    /// </summary>
    internal class Gameplay : Game
    {
        /// <summary>
        /// Пытается поставить метку текущего игрока в клетку поля.
        /// Если клетка занята, выводит сообщение и передаёт ход другому игроку.
        /// </summary>
        /// <param name="row">Номер строки (от 0 до 2).</param>
        /// <param name="col">Номер столбца (от 0 до 2).</param>
        /// <returns>
        /// <c>true</c>, если ход сделан; <c>false</c>, если клетка уже занята.
        /// </returns>
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

        /// <summary>
        /// Запускает игровой цикл: рисует поле, запрашивает строку и столбец,
        /// выполняет ход и проверяет состояние игры, пока партия не закончится
        /// победой или ничьей.
        /// </summary>
        public void Play()
        {
            while (gameStat == GameStat.InProcess)
            {
                DrawBoard.Draw(player1.map, player2.map);
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