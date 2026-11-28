using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphIncidentAdjacentIteratorTests
{
    [Fact] public void IncidentEdges_ContainsIncomingAndOutgoing() { var g = Build(out int a, out int b, out int c, out int d); g.AddEdge(a, b); g.AddEdge(c, a); g.AddEdge(a, d); Assert.Equal(new[] { (a, b), (a, d), (c, a) }, Read(g.GetConstIncidentEdgeIterator(a), e => (e.From.Id, e.To.Id))); }
    [Fact] public void IncidentEdges_ReverseIteratorReversesOrder() { var g = Build(out int a, out int b, out int c, out _); g.AddEdge(a, b); g.AddEdge(c, a); Assert.Equal(new[] { (c, a), (a, b) }, Read(g.GetConstReverseIncidentEdgeIterator(a), e => (e.From.Id, e.To.Id))); }
    [Fact] public void IncidentEdges_EmptyForIsolatedVertex() { var g = Build(out int a, out _, out _, out _); Assert.Empty(Read(g.GetConstIncidentEdgeIterator(a), e => e)); }
    [Fact] public void IncidentEdges_ThrowsForMissingVertex() { var g = new Graph<int>(); Assert.Throws<VertexNotFoundException>(() => g.GetConstIncidentEdgeIterator(1)); }
    [Fact] public void AdjacentVertices_CombinesIncomingAndOutgoingWithoutDuplicates() { var g = Build(out int a, out int b, out int c, out int d); g.AddEdge(a, b); g.AddEdge(b, a); g.AddEdge(c, a); g.AddEdge(a, d); Assert.Equal(new[] { b, c, d }, Read(g.GetConstAdjacentVertexIterator(a), v => v.Id)); }
    [Fact] public void AdjacentVertices_SelfLoopAppearsOnce() { var g = Build(out int a, out _, out _, out _); g.AddEdge(a, a); Assert.Equal(new[] { a }, Read(g.GetConstAdjacentVertexIterator(a), v => v.Id)); }
    [Fact] public void AdjacentVertices_ReverseIteratorReversesOrder() { var g = Build(out int a, out int b, out int c, out _); g.AddEdge(a, b); g.AddEdge(a, c); Assert.Equal(new[] { c, b }, Read(g.GetConstReverseAdjacentVertexIterator(a), v => v.Id)); }
    [Fact] public void AdjacentVertices_ThrowsForMissingVertex() { var g = new Graph<int>(); Assert.Throws<VertexNotFoundException>(() => g.GetConstAdjacentVertexIterator(5)); }
    [Fact] public void MutableIncidentAndAdjacentIteratorsAreAvailable() { var g = Build(out int a, out int b, out _, out _); g.AddEdge(a, b); Assert.IsType<GraphIterator<Edge<int>>>(g.GetIncidentEdgeIterator(a)); Assert.IsType<GraphIterator<Vertex<int>>>(g.GetAdjacentVertexIterator(a)); }
    private static Graph<int> Build(out int a, out int b, out int c, out int d) { var g = new Graph<int>(); a = g.AddVertex(0); b = g.AddVertex(1); c = g.AddVertex(2); d = g.AddVertex(3); return g; }
    private static List<TResult> Read<TItem, TResult>(GraphIteratorBase<TItem> it, Func<TItem, TResult> map) { var r = new List<TResult>(); GraphAlgorithms.ForEach(it, x => r.Add(map(x))); return r; }
}
