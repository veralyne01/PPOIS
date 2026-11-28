using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphRemovalTests
{
    [Fact] public void RemoveVertex_RemovesVertex() { var g = Build(out int a, out _); g.RemoveVertex(a); Assert.False(g.ContainsVertex(a)); Assert.Equal(1, g.VertexCount); }
    [Fact] public void RemoveVertex_RemovesOutgoingEdges() { var g = Build(out int a, out int b, out int c); g.AddEdge(a, b); g.AddEdge(a, c); g.RemoveVertex(a); Assert.Equal(0, g.EdgeCount); }
    [Fact] public void RemoveVertex_RemovesIncomingEdges() { var g = Build(out int a, out int b, out int c); g.AddEdge(b, a); g.AddEdge(c, a); g.RemoveVertex(a); Assert.Equal(0, g.EdgeCount); Assert.False(g.ContainsEdge(b, a)); }
    [Fact] public void RemoveVertex_RemovesIncomingAndOutgoingEdges() { var g = Build(out int a, out int b, out int c); g.AddEdge(a, b); g.AddEdge(b, a); g.AddEdge(a, c); g.AddEdge(c, a); g.RemoveVertex(a); Assert.Equal(0, g.EdgeCount); }
    [Fact] public void RemoveVertex_RemovesSelfLoopOnce() { var g = Build(out int a, out _); g.AddEdge(a, a); g.RemoveVertex(a); Assert.Equal(0, g.EdgeCount); }
    [Fact] public void RemoveVertex_RejectsUnknownId() { var g = new Graph<int>(); Assert.Throws<VertexNotFoundException>(() => g.RemoveVertex(10)); }
    [Fact] public void RemoveVertex_PreservesOtherEdges() { var g = Build(out int a, out int b, out int c); g.AddEdge(b, c); g.AddEdge(a, b); g.RemoveVertex(a); Assert.True(g.ContainsEdge(b, c)); Assert.Equal(1, g.EdgeCount); }
    [Fact] public void RemoveAndAdd_DoesNotReuseRemovedId() { var g = Build(out int a, out _); g.RemoveVertex(a); int id = g.AddVertex("new"); Assert.True(id > a); }
    [Fact] public void Clear_InvalidatesExistingIterator() { var g = Build(out _, out _); var it = g.GetVertexIterator(); g.Clear(); Assert.False(it.IsValid); Assert.Throws<InvalidIteratorException>(() => it.MoveNext()); }
    private static Graph<string> Build(out int a, out int b) { var g = new Graph<string>(); a = g.AddVertex("A"); b = g.AddVertex("B"); return g; }
    private static Graph<string> Build(out int a, out int b, out int c) { var g = Build(out a, out b); c = g.AddVertex("C"); return g; }
}
