using System;
using System.IO;
using System.Linq;

namespace TicTacToe
{
    /// <summary>
    /// Базовое состояние партии: два игрока, текущий игрок,
    /// статус игры и проверка победы или ничьей по битовым маскам.
    /// </summary>
    internal class Game
    {
        /// <summary>Первый игрок (X).</summary>
        protected Player player1;

        /// <summary>Второй игрок (O).</summary>
        protected Player player2;

        /// <summary>Текущее состояние партии.</summary>
        protected GameStat gameStat;

        /// <summary>Игрок, чей сейчас ход.</summary>
        protected Player curPlayer;

        /// <summary>
        /// Создаёт новую партию: игроков X и O, статус «в процессе»,
        /// первым ходит X.
        /// </summary>
        public Game()
        {
            player1 = new Player(Values.X);
            player2 = new Player(Values.O);
            gameStat = GameStat.InProcess;
            curPlayer = player1;
        }

        /// <summary>
        /// Выигрышные комбинации: каждая задана маской из 9 бит
        /// (три строки, три столбца и две диагонали).
        /// </summary>
        private readonly int[] winCombinations =
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

        /// <summary>
        /// Проверяет, выиграл ли текущий игрок или поле заполнено.
        /// При победе или ничьей обновляет статус игры, перерисовывает
        /// поле и выводит итоговое сообщение.
        /// </summary>
        protected void CheckGameStat()
        {
            if (winCombinations.Any(comb => (comb & curPlayer.map) == comb))
            {
                gameStat = GameStat.Win;
                try { Console.Clear(); }
                catch (IOException) { }
                DrawBoard.Draw(player1.map, player2.map);
                Console.WriteLine($"Player {curPlayer.Sign} wins!");
                Console.ReadLine();
            }
            else if ((player1.map | player2.map) == 0b111111111)
            {
                gameStat = GameStat.Draw;
                try { Console.Clear(); }
                catch (IOException) { }
                DrawBoard.Draw(player1.map, player2.map);
                Console.WriteLine("Friendship wins");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Передаёт ход другому игроку.
        /// </summary>
        protected void SwitchPlayers()
        {
            if (curPlayer == player1) curPlayer = player2;
            else curPlayer = player1;
        }
    }
}