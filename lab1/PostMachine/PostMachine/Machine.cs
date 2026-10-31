using System;
using System.Collections.Generic;

namespace PostMachine
{
    /// <summary>
    /// Интерпретатор программы машины Поста. Выполняет команды по номерам строк,
    /// управляя лентой и кареткой (<see cref="Slider"/>).
    /// </summary>
    public class Machine
    {
        private readonly Slider slider;
        private Dictionary<int, Command> program = new Dictionary<int, Command>();
        private readonly int startLine;

        /// <summary>Номер строки программы, которая будет выполнена следующей.</summary>
        public int CurrentLine { get; private set; }

        /// <summary>Признак того, что машина выполнила команду остановки.</summary>
        public bool IsStopped { get; private set; }

        /// <summary>
        /// Создаёт машину для заданной ленты.
        /// </summary>
        /// <param name="slider">Лента и каретка, которыми управляет программа.</param>
        /// <param name="startLine">Номер строки, с которой начинается выполнение.</param>
        public Machine(Slider slider, int startLine = 1)
        {
            this.slider = slider;
            this.startLine = startLine;
            CurrentLine = startLine;
        }

        /// <summary>
        /// Загружает программу и сбрасывает машину на стартовую строку.
        /// </summary>
        /// <param name="commands">Словарь «номер строки → команда» (см. <see cref="ProgramParser.Parse"/>).</param>
        public void Load(Dictionary<int, Command> commands)
        {
            program = commands;
            CurrentLine = startLine;
            IsStopped = false;
        }

        /// <summary>
        /// Выполняет одну команду текущей строки.
        /// </summary>
        /// <returns><c>false</c>, если машина остановилась; иначе <c>true</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Нет команды на текущей строке, метка ставится в уже помеченную клетку
        /// или стирается в пустой клетке.
        /// </exception>
        public bool Step()
        {
            if (IsStopped) return false;

            if (!program.TryGetValue(CurrentLine, out Command? cmd))
                throw new InvalidOperationException($"No command at line {CurrentLine}!\n");

            switch (cmd.Type)
            {
                case CommandType.Right:
                    slider.MoveRight(1);
                    CurrentLine = cmd.Next;
                    break;
                case CommandType.Left:
                    slider.MoveLeft(1);
                    CurrentLine = cmd.Next;
                    break;
                case CommandType.Mark:
                    // в классической машине Поста метить уже помеченную клетку нельзя
                    if (slider.CheckCell())
                        throw new InvalidOperationException($"Line {CurrentLine}: cell is already labelled!\n");
                    slider.MakeLabel();
                    CurrentLine = cmd.Next;
                    break;
                case CommandType.Erase:
                    if (!slider.CheckCell())
                        throw new InvalidOperationException($"Line {CurrentLine}: cell is already empty!\n");
                    slider.RemoveLabel();
                    CurrentLine = cmd.Next;
                    break;
                case CommandType.Check:
                    CurrentLine = slider.CheckCell() ? cmd.NextIfMarked : cmd.Next;
                    break;
                case CommandType.Stop:
                    IsStopped = true;
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Выполняет программу до команды остановки.
        /// </summary>
        /// <param name="maxSteps">Максимальное число шагов, защита от бесконечного цикла.</param>
        /// <exception cref="InvalidOperationException">
        /// Превышен лимит шагов или произошла ошибка выполнения команды (см. <see cref="Step"/>).
        /// </exception>
        public void Run(int maxSteps = 100000)
        {
            int steps = 0;
            while (Step())
            {
                if (++steps >= maxSteps)
                    throw new InvalidOperationException("Step limit exceeded, possible infinite loop!\n");
            }
        }
    }
}