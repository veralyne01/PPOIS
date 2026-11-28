namespace GeneralizedProgramming
{
    public readonly record struct Vertex<T>(int Id, T Value)
    {
        public override string ToString() => $"{Id}({Value})";
    }

    public readonly record struct Edge<T>(Vertex<T> From, Vertex<T> To)
    {
        public override string ToString() => $"{From} -> {To}";
    }
}