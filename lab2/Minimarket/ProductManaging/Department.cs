using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class Department
    {
        private int id;
        private static int nextDepId = 100;
        private string name;
        private Shelf[] shelves;
        public Department(string name)
        {
            this.name = name;
            id = ++nextDepId;
            shelves = new Shelf[5];
            for (int i = 1; i <= shelves.Length; i++)
            {
                shelves[i].number = i;
            }
        } 
    }
}
