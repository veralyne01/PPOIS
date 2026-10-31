namespace PostMachine
{
    /// <summary>
    /// Тип команды машины Поста.
    /// </summary>
    public enum CommandType
    {
        /// <summary>Сдвиг каретки на одну клетку вправо.</summary>
        Right,

        /// <summary>Сдвиг каретки на одну клетку влево.</summary>
        Left,

        /// <summary>Поставить метку в текущую клетку.</summary>
        Mark,

        /// <summary>Стереть метку в текущей клетке.</summary>
        Erase,

        /// <summary>Проверить клетку и перейти на одну из двух строк.</summary>
        Check,

        /// <summary>Остановить выполнение программы.</summary>
        Stop
    }

    /// <summary>
    /// Одна команда программы машины Поста: тип и номера строк для перехода.
    /// </summary>
    public class Command
    {
        /// <summary>Тип команды.</summary>
        public CommandType Type { get; }

        /// <summary>
        /// Номер следующей строки. Для <see cref="CommandType.Check"/>
        /// это переход, если клетка пуста.
        /// </summary>
        public int Next { get; }

        /// <summary>
        /// Номер следующей строки, если клетка помечена.
        /// Используется только командой <see cref="CommandType.Check"/>.
        /// </summary>
        public int NextIfMarked { get; }

        /// <summary>
        /// Создаёт команду.
        /// </summary>
        /// <param name="type">Тип команды.</param>
        /// <param name="next">Номер следующей строки (для проверки: если клетка пуста).</param>
        /// <param name="nextIfMarked">Номер следующей строки, если клетка помечена (только для проверки).</param>
        public Command(CommandType type, int next = 0, int nextIfMarked = 0)
        {
            Type = type;
            Next = next;
            NextIfMarked = nextIfMarked;
        }
    }
}