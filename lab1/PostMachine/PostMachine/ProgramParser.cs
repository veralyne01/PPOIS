using System;
using System.Collections.Generic;

namespace PostMachine
{
    /// <summary>
    /// Разбор текста программы машины Поста в набор команд.
    /// Формат строки: <c>номер команда аргументы</c>, например <c>2 ? 3 4</c>.
    /// </summary>
    public static class ProgramParser
    {
        /// <summary>
        /// Преобразует строки программы в словарь «номер строки → команда».
        /// Пустые строки пропускаются. Поддерживаемые команды:
        /// <c>R</c> или <c>&gt;</c> (вправо), <c>L</c> или <c>&lt;</c> (влево),
        /// <c>V</c> (поставить метку), <c>X</c> (стереть метку),
        /// <c>?</c> (проверка клетки, два перехода), <c>!</c> (стоп).
        /// </summary>
        /// <param name="lines">Строки программы.</param>
        /// <returns>Словарь, где ключ это номер строки программы.</returns>
        /// <exception cref="FormatException">
        /// Неверный формат строки, номер строки меньше 1 или повторяется,
        /// неизвестная команда, отсутствует или неверен аргумент перехода.
        /// </exception>
        public static Dictionary<int, Command> Parse(IEnumerable<string> lines)
        {
            var result = new Dictionary<int, Command>();

            foreach (string raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0) continue;

                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || !int.TryParse(parts[0], out int number) || number < 1)
                    throw new FormatException($"Invalid line: \"{raw}\"\n");
                if (result.ContainsKey(number))
                    throw new FormatException($"Duplicate line number {number}!\n");

                Command cmd;
                switch (parts[1].ToUpperInvariant())
                {
                    case "R":
                    case ">":
                        cmd = new Command(CommandType.Right, Arg(parts, 2, raw));
                        break;
                    case "L":
                    case "<":
                        cmd = new Command(CommandType.Left, Arg(parts, 2, raw));
                        break;
                    case "V":
                        cmd = new Command(CommandType.Mark, Arg(parts, 2, raw));
                        break;
                    case "X":
                        cmd = new Command(CommandType.Erase, Arg(parts, 2, raw));
                        break;
                    case "?":
                        cmd = new Command(CommandType.Check, Arg(parts, 2, raw), Arg(parts, 3, raw));
                        break;
                    case "!":
                        cmd = new Command(CommandType.Stop);
                        break;
                    default:
                        throw new FormatException($"Unknown command in line: \"{raw}\"\n");
                }
                result[number] = cmd;
            }
            return result;
        }

        /// <summary>
        /// Читает числовой аргумент команды (номер строки перехода).
        /// </summary>
        /// <param name="parts">Слова текущей строки программы.</param>
        /// <param name="index">Индекс нужного аргумента в <paramref name="parts"/>.</param>
        /// <param name="raw">Исходная строка, используется в тексте ошибки.</param>
        /// <returns>Номер строки перехода (не меньше 1).</returns>
        /// <exception cref="FormatException">Аргумент отсутствует, не число или меньше 1.</exception>
        private static int Arg(string[] parts, int index, string raw)
        {
            if (index >= parts.Length || !int.TryParse(parts[index], out int value) || value < 1)
                throw new FormatException($"Invalid or missing argument in line: \"{raw}\"\n");
            return value;
        }
    }
}