using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PostMachine
{
    /// <summary>
    /// Пошаговая визуализация работы машины Поста в консоли.
    /// </summary>
    public static class ConsoleVisualizer
    {
        /// <summary>
        /// Рисует участок ленты вокруг каретки: стрелка над текущей клеткой,
        /// <c>#</c> в клетках с меткой и номера клеток под лентой.
        /// </summary>
        /// <param name="slider">Лента и каретка.</param>
        /// <param name="radius">Сколько клеток показывать слева и справа от каретки.</param>
        public static void DrawTape(Slider slider, int radius = 7)
        {
            int pos = slider.Position;
            var labels = new HashSet<int>(slider.GetLabels());
            var caret = new StringBuilder();
            var cells = new StringBuilder();
            var index = new StringBuilder();

            for (int p = pos - radius; p <= pos + radius; p++)
            {
                caret.Append(p == pos ? "  v  " : "     ");
                cells.Append(labels.Contains(p) ? "[ # ]" : "[   ]");
                index.Append(p.ToString().PadLeft(4).PadRight(5));
            }

            Console.WriteLine(caret);
            Console.WriteLine(cells);
            Console.WriteLine(index);
        }

        /// <summary>
        /// Выполняет программу и после каждого шага выводит ленту.
        /// </summary>
        /// <param name="slider">Лента и каретка.</param>
        /// <param name="program">Разобранная программа.</param>
        /// <param name="delayMs">Пауза между шагами в миллисекундах (0 или перенаправленный вывод: без паузы).</param>
        /// <param name="maxSteps">Максимальное число шагов, защита от бесконечного цикла.</param>
        /// <exception cref="InvalidOperationException">
        /// Превышен лимит шагов или ошибка выполнения команды.
        /// </exception>
        public static void Run(Slider slider, Dictionary<int, Command> program,
                               int delayMs = 500, int maxSteps = 1000)
        {
            var machine = new Machine(slider);
            machine.Load(program);

            Console.WriteLine("Start");
            DrawTape(slider);

            int step = 0;
            while (true)
            {
                int line = machine.CurrentLine;
                bool goOn = machine.Step();
                step++;

                Console.WriteLine($"\nStep {step}: line {line}");
                DrawTape(slider);

                if (!goOn)
                {
                    Console.WriteLine("\nStopped");
                    return;
                }
                if (step >= maxSteps)
                    throw new InvalidOperationException("Step limit exceeded, possible infinite loop!\n");

                if (delayMs > 0 && !Console.IsOutputRedirected)
                    Thread.Sleep(delayMs);
            }
        }
    }
}