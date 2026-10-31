using System;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("TicTacToe_Tests")]

namespace TicTacToe
{
    /// <summary>
    /// Точка входа консольной игры «Крестики-нолики».
    /// </summary>
    internal class Start
    {
        /// <summary>
        /// Создаёт партию и запускает игровой цикл.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        internal static void Main(string[] args)
        {
            Gameplay game = new Gameplay();
            game.Play();
        }
    }
}