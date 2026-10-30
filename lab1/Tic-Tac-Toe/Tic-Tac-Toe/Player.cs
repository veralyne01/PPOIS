using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Tests")]

namespace TicTacToe
{
    internal class Player
    {
        internal int map;
        internal Values Sign;
        internal Player(Values sign)
        {
            map = 0;
            Sign = sign;
        }
    }
}