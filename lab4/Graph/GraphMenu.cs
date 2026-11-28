namespace GeneralizedProgramming
{
    /// <summary>Консольное меню для демонстрации контейнера Graph&lt;string&gt;.</summary>
    public sealed class GraphMenu
    {
        private readonly Graph<string> _graph = new Graph<string>();

        public void Run()
        {
            while (true)
            {
                PrintMenu();
                string choice = Prompt("Choice").Trim();
                if (choice == "0") return;

                try
                {
                    Execute(choice);
                }
                catch (GraphException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                Console.WriteLine();
            }
        }

        private void Execute(string choice)
        {
            switch (choice)
            {
                case "1": AddVertex(); break;
                case "2": AddEdge(); break;
                case "3": RemoveVertex(); break;
                case "4": RemoveEdge(); break;
                case "5": ContainsVertex(); break;
                case "6": ContainsEdge(); break;
                case "7": ShowCounts(); break;
                case "8": ShowVertexDegrees(); break;
                case "9": ShowEdgeDegree(); break;
                case "10": TraverseVertices(); break;
                case "11": TraverseEdges(); break;
                case "12": TraverseIncidentEdges(); break;
                case "13": TraverseAdjacentVertices(); break;
                case "14": RemoveVertexViaIterator(); break;
                case "15": RemoveEdgeViaIterator(); break;
                case "16": CopyAndCompare(); break;
                case "17": FillDemo(); break;
                case "18": Console.WriteLine(_graph); break;
                case "19": _graph.Clear(); Console.WriteLine("Graph cleaned."); break;
                default: Console.WriteLine("Error."); break;
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("=== Directed graph (ordered edge lists) ===");
            Console.WriteLine(" 1. Add vertex");
            Console.WriteLine(" 2. Add edge");
            Console.WriteLine(" 3. Remove vertex (by id)");
            Console.WriteLine(" 4. Remove edge (by source and target id)");
            Console.WriteLine(" 5. Check if vertex exists");
            Console.WriteLine(" 6. Check if edge exists");
            Console.WriteLine(" 7. Vertex and edge counts, is graph empty");
            Console.WriteLine(" 8. Vertex degrees (in, out, total)");
            Console.WriteLine(" 9. Edge degree");
            Console.WriteLine("10. Traverse vertices with an iterator");
            Console.WriteLine("11. Traverse edges with an iterator");
            Console.WriteLine("12. Traverse edges incident to a vertex");
            Console.WriteLine("13. Traverse vertices adjacent to a vertex");
            Console.WriteLine("14. Remove vertex via iterator");
            Console.WriteLine("15. Remove edge via iterator");
            Console.WriteLine("16. Copy and compare graphs");
            Console.WriteLine("17. Add demo data");
            Console.WriteLine("18. Show graph");
            Console.WriteLine("19. Clear graph");
            Console.WriteLine(" 0. Exit");
        }

        private void AddVertex()
        {
            string value = Prompt("Value: ");
            int id = _graph.AddVertex(value);
            Console.WriteLine($"Add vertex {_graph.GetVertex(id)}.");
        }

        private void AddEdge()
        {
            int from = ReadInt("edge beginning id");
            int to = ReadInt("edge ending id");
            _graph.AddEdge(from, to);
            Console.WriteLine($"Add edge {from} -> {to}.");
        }

        private void RemoveVertex()
        {
            int id = ReadInt("vertex id");
            _graph.RemoveVertex(id);
            Console.WriteLine($"Vertex {id} was deleted along with the incident edges.");
        }

        private void RemoveEdge()
        {
            int from = ReadInt("edge beginning id");
            int to = ReadInt("edge ending id");
            _graph.RemoveEdge(from, to);
            Console.WriteLine($"Edge {from} -> {to} deleted.");
        }

        private void ContainsVertex()
        {
            int id = ReadInt("vertex id");
            Console.WriteLine(_graph.ContainsVertex(id) ? "Vertex found." : "Vertex not found.");
        }

        private void ContainsEdge()
        {
            int from = ReadInt("edge beginning id");
            int to = ReadInt("edge ending id");
            Console.WriteLine(_graph.ContainsEdge(from, to) ? "Edge found." : "Edge not found.");
        }

        private void ShowCounts()
        {
            Console.WriteLine($"Vertices: {_graph.VertexCount}, edges: {_graph.EdgeCount}, empty: {(_graph.IsEmpty ? "yes" : "no")}.");
        }

        private void ShowVertexDegrees()
        {
            int id = ReadInt("vertex id");
            Console.WriteLine($"Incoming: {_graph.InDegree(id)}, outcoming: {_graph.OutDegree(id)}, common: {_graph.Degree(id)}.");
        }

        private void ShowEdgeDegree()
        {
            int from = ReadInt("edge beginning id");
            int to = ReadInt("edge ending id");
            Console.WriteLine($"Edge degree {from} -> {to}: {_graph.EdgeDegree(from, to)}.");
        }

        private void TraverseVertices()
        {
            if (ReadReverse())
                Traverse(_graph.GetConstReverseVertexIterator());
            else
                Traverse(_graph.GetConstVertexIterator());
        }

        private void TraverseEdges()
        {
            if (ReadReverse())
                Traverse(_graph.GetConstReverseEdgeIterator());
            else
                Traverse(_graph.GetConstEdgeIterator());
        }

        private void TraverseIncidentEdges()
        {
            int id = ReadInt("vertex id");
            if (ReadReverse())
                Traverse(_graph.GetConstReverseIncidentEdgeIterator(id));
            else
                Traverse(_graph.GetConstIncidentEdgeIterator(id));
        }

        private void TraverseAdjacentVertices()
        {
            int id = ReadInt("vertex id");
            if (ReadReverse())
                Traverse(_graph.GetConstReverseAdjacentVertexIterator(id));
            else
                Traverse(_graph.GetConstAdjacentVertexIterator(id));
        }

        private static void Traverse<TItem>(ConstGraphIterator<TItem> iterator)
        {
            var forward = new List<string>();
            GraphAlgorithms.ForEach(iterator, item => forward.Add(item?.ToString() ?? string.Empty));
            Console.WriteLine($"Forward (MoveNext): {JoinOrDash(forward)}");

            var backward = new List<string>();
            while (iterator.MovePrev())
                backward.Add(iterator.Current?.ToString() ?? string.Empty);
            Console.WriteLine($"Back  (MovePrev): {JoinOrDash(backward)}");
        }

        private void RemoveVertexViaIterator()
        {
            if (_graph.VertexCount == 0)
            {
                Console.WriteLine("No vertices found.");
                return;
            }

            PrintNumbered(_graph.GetConstVertexIterator());
            int number = ReadInt("Vertex number");
            if (number < 1 || number > _graph.VertexCount)
            {
                Console.WriteLine("Number is out of range.");
                return;
            }

            GraphIterator<Vertex<string>> iterator = _graph.GetVertexIterator();
            for (int i = 0; i < number; i++) iterator.MoveNext();

            Vertex<string> target = iterator.Current;
            _graph.RemoveVertex(iterator);
            Console.WriteLine($"Delete vertex {target}.");
        }

        private void RemoveEdgeViaIterator()
        {
            if (_graph.EdgeCount == 0)
            {
                Console.WriteLine("No edges found.");
                return;
            }

            PrintNumbered(_graph.GetConstEdgeIterator());
            int number = ReadInt("Edge number");
            if (number < 1 || number > _graph.EdgeCount)
            {
                Console.WriteLine("Number is out of range.");
                return;
            }

            GraphIterator<Edge<string>> iterator = _graph.GetEdgeIterator();
            for (int i = 0; i < number; i++) iterator.MoveNext();

            Edge<string> target = iterator.Current;
            _graph.RemoveEdge(iterator);
            Console.WriteLine($"Delete edge {target}.");
        }

        private static void PrintNumbered<TItem>(ConstGraphIterator<TItem> iterator)
        {
            int n = 0;
            GraphAlgorithms.ForEach(iterator, item => Console.WriteLine($"  {++n}. {item}"));
        }

        private void CopyAndCompare()
        {
            Graph<string> copy = _graph.Clone();
            Console.WriteLine($"Clone created. copy == graph: {copy == _graph}");

            copy.AddVertex("extra");
            Console.WriteLine("Add vertex to clone \"extra\":");
            Console.WriteLine($"  ==: {copy == _graph}, !=: {copy != _graph}");
            Console.WriteLine($"  <: {copy < _graph}, >: {copy > _graph}, <=: {copy <= _graph}, >=: {copy >= _graph}");

            var assigned = new Graph<string>();
            assigned.CopyFrom(copy);
            Console.WriteLine($"CopyFrom: assigned == copy: {assigned == copy}");
            Console.WriteLine("Clone:");
            Console.WriteLine(assigned);
        }

        private void FillDemo()
        {
            int a = _graph.AddVertex("A");
            int b = _graph.AddVertex("B");
            int c = _graph.AddVertex("C");
            int d = _graph.AddVertex("D");

            _graph.AddEdge(a, b);
            _graph.AddEdge(a, c);
            _graph.AddEdge(b, c);
            _graph.AddEdge(c, a);
            _graph.AddEdge(c, d);

            Console.WriteLine("Add 4 vertices (A, B, C, D) and 5 edges.");
            Console.WriteLine(_graph);
        }

        private static string Prompt(string text)
        {
            Console.Write($"{text}: ");
            return Console.ReadLine() ?? "0";
        }

        private static int ReadInt(string text)
        {
            while (true)
            {
                string input = Prompt(text);
                if (int.TryParse(input, out int value)) return value;
                Console.WriteLine("Enter number.");
            }
        }

        private static bool ReadReverse()
        {
            while (true)
            {
                string input = Prompt("Direction (1 - straight, 2 - reverse)").Trim();
                if (input == "1") return false;
                if (input == "2") return true;
                Console.WriteLine("Enter 1 or 2.");
            }
        }

        private static string JoinOrDash(List<string> items)
            => items.Count == 0 ? "-" : string.Join(", ", items);
    }
}