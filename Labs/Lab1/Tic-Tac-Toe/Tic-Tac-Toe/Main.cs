using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]

namespace TicTacToe
{
    class Start
    {
        private static void Main(string[] args)
        {
            Game game = new Game();
            game.NewGame();
            game.Play();
        }
    }
}