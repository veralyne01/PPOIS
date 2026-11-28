namespace GeneralizedProgramming
{
    /// <summary>Вершина графа: идентификатор и хранимое значение.</summary>
    public readonly record struct Vertex<T>(int Id, T Value)
    {
        public override string ToString() => $"{Id}({Value})";
    }

    /// <summary>Ориентированное ребро From -> To (пара вершин).</summary>
    public readonly record struct Edge<T>(Vertex<T> From, Vertex<T> To)
    {
        public override string ToString() => $"{From} -> {To}";
    }
}