namespace GeneralizedProgramming
{
    /// <summary>Базовое исключение контейнера Graph.</summary>
    public class GraphException : Exception
    {
        public GraphException(string message) : base(message) { }
    }

    public class VertexNotFoundException : GraphException
    {
        public int VertexId { get; }

        public VertexNotFoundException(int id)
            : base($"Вершина с id = {id} не найдена.")
        {
            VertexId = id;
        }
    }

    public class EdgeNotFoundException : GraphException
    {
        public int From { get; }
        public int To { get; }

        public EdgeNotFoundException(int from, int to)
            : base($"Ребро {from} -> {to} не найдено.")
        {
            From = from;
            To = to;
        }
    }

    public class EdgeAlreadyExistsException : GraphException
    {
        public int From { get; }
        public int To { get; }

        public EdgeAlreadyExistsException(int from, int to)
            : base($"Ребро {from} -> {to} уже существует.")
        {
            From = from;
            To = to;
        }
    }

    public class InvalidIteratorException : GraphException
    {
        public InvalidIteratorException(string message) : base(message) { }
    }
}