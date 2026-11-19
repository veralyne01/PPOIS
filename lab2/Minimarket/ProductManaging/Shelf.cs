using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class Shelf
    {
        private int id;
        private static int nextShelfId = 200;
        public int number;
        private int capacity = 50;
        Shelf()
        {
            id = ++nextShelfId;
        }
    }
}
