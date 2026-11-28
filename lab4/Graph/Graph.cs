using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralizedProgramming
{
    public sealed class Graph<T> : IGraphOwner, IEquatable<Graph<T>>, IComparable<Graph<T>>
        where T : IComparable<T>
    {
        private sealed class VertexNode
        {
            public int Id;
            public T Value;
            public List<int> Out = new List<int>();

            public VertexNode(int id, T value)
            {
                Id = id;
                Value = value;
            }
        }
        private readonly List<VertexNode> _vertices = new List<VertexNode>();
        private int _nextId;
        private int _edgeCount;
        private int _version;

        int IGraphOwner.Version => _version;

        public Graph() { }

        public Graph(Graph<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            CopyData(other);
        }

        public Graph<T> Clone() => new Graph<T>(this);

        public void CopyFrom(Graph<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (ReferenceEquals(this, other)) return;
            CopyData(other);
            _version++;
        }

        private void CopyData(Graph<T> other)
        {
            _vertices.Clear();
            foreach (VertexNode n in other._vertices)
            {
                var copy = new VertexNode(n.Id, n.Value);
                copy.Out.AddRange(n.Out);
                _vertices.Add(copy);
            }
            _nextId = other._nextId;
            _edgeCount = other._edgeCount;
        }

        public bool IsEmpty => _vertices.Count == 0;

        public void Clear()
        {
            _vertices.Clear();
            _edgeCount = 0;
            _nextId = 0;
            _version++;
        }

        public int VertexCount => _vertices.Count;

        public int EdgeCount => _edgeCount;

        private int FindIndex(int id)
        {
            int lo = 0, hi = _vertices.Count - 1;
            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;
                int cur = _vertices[mid].Id;
                if (cur == id) return mid;
                if (cur < id) lo = mid + 1;
                else hi = mid - 1;
            }
            return -1;
        }

        private VertexNode GetNode(int id)
        {
            int idx = FindIndex(id);
            if (idx < 0) throw new VertexNotFoundException(id);
            return _vertices[idx];
        }

        private static Vertex<T> ToVertex(VertexNode n) => new Vertex<T>(n.Id, n.Value);

        public bool ContainsVertex(int id) => FindIndex(id) >= 0;

        public bool ContainsEdge(int from, int to)
        {
            int idx = FindIndex(from);
            return idx >= 0 && _vertices[idx].Out.BinarySearch(to) >= 0;
        }

        public Vertex<T> GetVertex(int id) => ToVertex(GetNode(id));

        public int OutDegree(int id) => GetNode(id).Out.Count;

        public int InDegree(int id)
        {
            GetNode(id);
            int count = 0;
            foreach (VertexNode n in _vertices)
                if (n.Out.BinarySearch(id) >= 0) count++;
            return count;
        }

        public int Degree(int id) => InDegree(id) + OutDegree(id);

        public int EdgeDegree(int from, int to)
        {
            GetNode(from);
            GetNode(to);
            if (!ContainsEdge(from, to)) throw new EdgeNotFoundException(from, to);

            var set = new HashSet<(int, int)>();
            foreach ((int, int) e in CollectIncidentEdges(from)) set.Add(e);
            foreach ((int, int) e in CollectIncidentEdges(to)) set.Add(e);
            set.Remove((from, to));
            return set.Count;
        }

        // ---------- Добавление ----------

        /// <summary>Добавляет вершину и возвращает её id.</summary>
        public int AddVertex(T value)
        {
            int id = _nextId++;
            _vertices.Add(new VertexNode(id, value));
            _version++;
            return id;
        }

        public void AddEdge(int from, int to)
        {
            VertexNode src = GetNode(from);
            GetNode(to);

            int pos = src.Out.BinarySearch(to);
            if (pos >= 0) throw new EdgeAlreadyExistsException(from, to);

            src.Out.Insert(~pos, to);
            _edgeCount++;
            _version++;
        }

        // ---------- Удаление ----------

        public void RemoveEdge(int from, int to)
        {
            VertexNode src = GetNode(from);
            GetNode(to);

            int pos = src.Out.BinarySearch(to);
            if (pos < 0) throw new EdgeNotFoundException(from, to);

            src.Out.RemoveAt(pos);
            _edgeCount--;
            _version++;
        }

        public void RemoveVertex(int id)
        {
            int idx = FindIndex(id);
            if (idx < 0) throw new VertexNotFoundException(id);

            _edgeCount -= _vertices[idx].Out.Count;
            _vertices.RemoveAt(idx);

            foreach (VertexNode n in _vertices)
            {
                int pos = n.Out.BinarySearch(id);
                if (pos >= 0)
                {
                    n.Out.RemoveAt(pos);
                    _edgeCount--;
                }
            }
            _version++;
        }

        /// <summary>Удаление вершины по итератору. После удаления итератор недействителен.</summary>
        public void RemoveVertex(GraphIterator<Vertex<T>> iterator)
        {
            CheckIterator(iterator);
            RemoveVertex(iterator.Current.Id);
        }

        /// <summary>Удаление ребра по итератору. После удаления итератор недействителен.</summary>
        public void RemoveEdge(GraphIterator<Edge<T>> iterator)
        {
            CheckIterator(iterator);
            Edge<T> e = iterator.Current;
            RemoveEdge(e.From.Id, e.To.Id);
        }

        private void CheckIterator<TItem>(GraphIteratorBase<TItem> iterator)
        {
            if (iterator == null) throw new ArgumentNullException(nameof(iterator));
            if (!ReferenceEquals(iterator.Owner, this))
                throw new InvalidIteratorException("Итератор принадлежит другому графу.");
            // Current сам бросит исключение, если итератор недействителен или не указывает на элемент
            _ = iterator.Current;
        }

        // ---------- Снимки для итераторов ----------

        private List<(int From, int To)> CollectIncidentEdges(int v)
        {
            var result = new List<(int From, int To)>();
            foreach (VertexNode n in _vertices)
            {
                if (n.Id == v)
                {
                    foreach (int t in n.Out) result.Add((n.Id, t));
                }
                else if (n.Out.BinarySearch(v) >= 0)
                {
                    result.Add((n.Id, v));
                }
            }
            return result;
        }

        private Edge<T> MakeEdge(int from, int to) => new Edge<T>(GetVertex(from), GetVertex(to));

        private List<Vertex<T>> SnapshotVertices()
        {
            var list = new List<Vertex<T>>(_vertices.Count);
            foreach (VertexNode n in _vertices) list.Add(ToVertex(n));
            return list;
        }

        private List<Edge<T>> SnapshotEdges()
        {
            var list = new List<Edge<T>>(_edgeCount);
            foreach (VertexNode n in _vertices)
                foreach (int t in n.Out)
                    list.Add(new Edge<T>(ToVertex(n), GetVertex(t)));
            return list;
        }

        private List<Edge<T>> SnapshotIncidentEdges(int v)
        {
            GetNode(v);
            var list = new List<Edge<T>>();
            foreach ((int from, int to) in CollectIncidentEdges(v))
                list.Add(MakeEdge(from, to));
            return list;
        }

        private List<Vertex<T>> SnapshotAdjacentVertices(int v)
        {
            VertexNode node = GetNode(v);
            var ids = new SortedSet<int>(node.Out);
            foreach (VertexNode n in _vertices)
                if (n.Out.BinarySearch(v) >= 0) ids.Add(n.Id);

            var list = new List<Vertex<T>>(ids.Count);
            foreach (int id in ids) list.Add(GetVertex(id));
            return list;
        }

        private static List<TItem> Reversed<TItem>(List<TItem> list)
        {
            list.Reverse();
            return list;
        }

        // ---------- Итераторы по вершинам ----------

        public GraphIterator<Vertex<T>> GetVertexIterator()
            => new GraphIterator<Vertex<T>>(this, SnapshotVertices());

        public GraphIterator<Vertex<T>> GetReverseVertexIterator()
            => new GraphIterator<Vertex<T>>(this, Reversed(SnapshotVertices()));

        public ConstGraphIterator<Vertex<T>> GetConstVertexIterator()
            => new ConstGraphIterator<Vertex<T>>(this, SnapshotVertices());

        public ConstGraphIterator<Vertex<T>> GetConstReverseVertexIterator()
            => new ConstGraphIterator<Vertex<T>>(this, Reversed(SnapshotVertices()));

        // ---------- Итераторы по рёбрам ----------

        public GraphIterator<Edge<T>> GetEdgeIterator()
            => new GraphIterator<Edge<T>>(this, SnapshotEdges());

        public GraphIterator<Edge<T>> GetReverseEdgeIterator()
            => new GraphIterator<Edge<T>>(this, Reversed(SnapshotEdges()));

        public ConstGraphIterator<Edge<T>> GetConstEdgeIterator()
            => new ConstGraphIterator<Edge<T>>(this, SnapshotEdges());

        public ConstGraphIterator<Edge<T>> GetConstReverseEdgeIterator()
            => new ConstGraphIterator<Edge<T>>(this, Reversed(SnapshotEdges()));

        // ---------- Итераторы по рёбрам, инцидентным вершине ----------

        public GraphIterator<Edge<T>> GetIncidentEdgeIterator(int vertexId)
            => new GraphIterator<Edge<T>>(this, SnapshotIncidentEdges(vertexId));

        public GraphIterator<Edge<T>> GetReverseIncidentEdgeIterator(int vertexId)
            => new GraphIterator<Edge<T>>(this, Reversed(SnapshotIncidentEdges(vertexId)));

        public ConstGraphIterator<Edge<T>> GetConstIncidentEdgeIterator(int vertexId)
            => new ConstGraphIterator<Edge<T>>(this, SnapshotIncidentEdges(vertexId));

        public ConstGraphIterator<Edge<T>> GetConstReverseIncidentEdgeIterator(int vertexId)
            => new ConstGraphIterator<Edge<T>>(this, Reversed(SnapshotIncidentEdges(vertexId)));

        // ---------- Итераторы по смежным вершинам ----------

        public GraphIterator<Vertex<T>> GetAdjacentVertexIterator(int vertexId)
            => new GraphIterator<Vertex<T>>(this, SnapshotAdjacentVertices(vertexId));

        public GraphIterator<Vertex<T>> GetReverseAdjacentVertexIterator(int vertexId)
            => new GraphIterator<Vertex<T>>(this, Reversed(SnapshotAdjacentVertices(vertexId)));

        public ConstGraphIterator<Vertex<T>> GetConstAdjacentVertexIterator(int vertexId)
            => new ConstGraphIterator<Vertex<T>>(this, SnapshotAdjacentVertices(vertexId));

        public ConstGraphIterator<Vertex<T>> GetConstReverseAdjacentVertexIterator(int vertexId)
            => new ConstGraphIterator<Vertex<T>>(this, Reversed(SnapshotAdjacentVertices(vertexId)));

        // ---------- Сравнение ----------

        /// <summary>Графы равны, если совпадают вершины (id и значения) и рёбра.</summary>
        public bool Equals(Graph<T>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (_vertices.Count != other._vertices.Count || _edgeCount != other._edgeCount)
                return false;

            for (int i = 0; i < _vertices.Count; i++)
            {
                VertexNode a = _vertices[i];
                VertexNode b = other._vertices[i];
                if (a.Id != b.Id) return false;
                if (!EqualityComparer<T>.Default.Equals(a.Value, b.Value)) return false;
                if (!a.Out.SequenceEqual(b.Out)) return false;
            }
            return true;
        }

        /// <summary>
        /// Порядок: число вершин, число рёбер, затем лексикографически
        /// (id вершины, значение, список исходящих рёбер).
        /// </summary>
        public int CompareTo(Graph<T>? other)
        {
            if (other is null) return 1;
            if (ReferenceEquals(this, other)) return 0;

            int c = _vertices.Count.CompareTo(other._vertices.Count);
            if (c != 0) return c;

            c = _edgeCount.CompareTo(other._edgeCount);
            if (c != 0) return c;

            for (int i = 0; i < _vertices.Count; i++)
            {
                VertexNode a = _vertices[i];
                VertexNode b = other._vertices[i];

                c = a.Id.CompareTo(b.Id);
                if (c != 0) return c;

                c = Comparer<T>.Default.Compare(a.Value, b.Value);
                if (c != 0) return c;

                int common = Math.Min(a.Out.Count, b.Out.Count);
                for (int j = 0; j < common; j++)
                {
                    c = a.Out[j].CompareTo(b.Out[j]);
                    if (c != 0) return c;
                }

                c = a.Out.Count.CompareTo(b.Out.Count);
                if (c != 0) return c;
            }
            return 0;
        }

        public override bool Equals(object? obj) => Equals(obj as Graph<T>);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(_vertices.Count);
            foreach (VertexNode n in _vertices)
            {
                hash.Add(n.Id);
                hash.Add(n.Value);
                foreach (int t in n.Out) hash.Add(t);
            }
            return hash.ToHashCode();
        }

        private static int Compare(Graph<T>? left, Graph<T>? right)
        {
            if (left is null) return right is null ? 0 : -1;
            return left.CompareTo(right);
        }

        public static bool operator ==(Graph<T>? left, Graph<T>? right)
            => left is null ? right is null : left.Equals(right);

        public static bool operator !=(Graph<T>? left, Graph<T>? right) => !(left == right);

        public static bool operator <(Graph<T>? left, Graph<T>? right) => Compare(left, right) < 0;

        public static bool operator >(Graph<T>? left, Graph<T>? right) => Compare(left, right) > 0;

        public static bool operator <=(Graph<T>? left, Graph<T>? right) => Compare(left, right) <= 0;

        public static bool operator >=(Graph<T>? left, Graph<T>? right) => Compare(left, right) >= 0;


        public override string ToString()
        {
            var vertices = new List<string>();
            GraphAlgorithms.ForEach(GetConstVertexIterator(), v => vertices.Add(v.ToString()));

            var edges = new List<string>();
            GraphAlgorithms.ForEach(GetConstEdgeIterator(), e => edges.Add(e.ToString()));

            string vText = vertices.Count == 0 ? "-" : string.Join(", ", vertices);
            string eText = edges.Count == 0 ? "-" : string.Join(", ", edges);

            return $"Вершины ({VertexCount}): {vText}{Environment.NewLine}Рёбра ({EdgeCount}): {eText}";
        }
    }
}