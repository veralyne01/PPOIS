using System.Collections.Generic;

namespace PostMachine
{
    public class Slider
    {
        private int position = 0;
        public int Position
        {
            get { return position; }
            private set { position = value; }
        }
        private HashSet<int> tape = new HashSet<int>();
        public void MakeLabel()
        {
            tape.Add(position);
        }
        public void RemoveLabel()
        {
            tape.Remove(position);
        }
        public void MoveRight(int step)
        {
            position += step;
        }
        public void MoveLeft(int step)
        {
            position -= step;
        }
        public bool CheckCell()
        {
            if (tape.Contains(position)) return true;
            return false;
        }
        public void Reset()
        {
            tape.Clear();
            position = 0;
        }
    }
}