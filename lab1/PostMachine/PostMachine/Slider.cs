using System.Collections.Generic;

namespace PostMachine
{
    /// <summary>
    /// Лента и каретка машины Поста. Хранит позиции помеченных клеток
    /// и текущее положение каретки. Лента бесконечна в обе стороны.
    /// </summary>
    public class Slider
    {
        private int position = 0;

        /// <summary>
        /// Текущая позиция каретки. Может быть отрицательной.
        /// </summary>
        public int Position
        {
            get { return position; }
            private set { position = value; }
        }

        /// <summary>Номера клеток, в которых стоит метка.</summary>
        private HashSet<int> tape = new HashSet<int>();

        /// <summary>
        /// Ставит метку в текущую клетку. Если метка уже есть, ничего не меняется.
        /// </summary>
        public void MakeLabel()
        {
            tape.Add(position);
        }

        /// <summary>
        /// Убирает метку из текущей клетки. Если клетка пуста, ничего не меняется.
        /// </summary>
        public void RemoveLabel()
        {
            tape.Remove(position);
        }

        /// <summary>
        /// Сдвигает каретку вправо.
        /// </summary>
        /// <param name="step">Количество клеток, на которое сдвигается каретка.</param>
        public void MoveRight(int step)
        {
            position += step;
        }

        /// <summary>
        /// Сдвигает каретку влево.
        /// </summary>
        /// <param name="step">Количество клеток, на которое сдвигается каретка.</param>
        public void MoveLeft(int step)
        {
            position -= step;
        }

        /// <summary>
        /// Проверяет, есть ли метка в текущей клетке.
        /// </summary>
        /// <returns><c>true</c>, если клетка помечена; иначе <c>false</c>.</returns>
        public bool CheckCell()
        {
            if (tape.Contains(position)) return true;
            return false;
        }

        /// <summary>
        /// Создаёт новую пустую ленту: стирает все метки и возвращает каретку в позицию 0.
        /// </summary>
        public void Reset()
        {
            tape.Clear();
            position = 0;
        }
    }
}