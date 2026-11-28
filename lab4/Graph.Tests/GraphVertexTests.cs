using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphVertexTests
{
    [Fact] public void NewGraph_IsEmpty() { var g = new Graph<string>(); Assert.True(g.IsEmpty); Assert.Equal(0, g.VertexCount); Assert.Equal(0, g.EdgeCount); }
    [Fact] public void AddVertex_ReturnsIncreasingIds() { var g = new Graph<string>(); Assert.Equal(0, g.AddVertex("A")); Assert.Equal(1, g.AddVertex("B")); Assert.Equal(2, g.AddVertex("C")); }
    [Fact] public void AddVertex_AllowsDuplicateAndNullValues() { var g = new Graph<string>(); var a = g.AddVertex("same"); var b = g.AddVertex("same"); var c = g.AddVertex(null!); Assert.NotEqual(a, b); Assert.Equal("same", g.GetVertex(b).Value); Assert.Null(g.GetVertex(c).Value); }
    [Fact] public void GetVertex_ReturnsIdAndValue() { var g = new Graph<string>(); int id = g.AddVertex("node"); Assert.Equal(new Vertex<string>(id, "node"), g.GetVertex(id)); }
    [Fact] public void ContainsVertex_ReturnsFalseForMissingIds() { var g = new Graph<int>(); g.AddVertex(10); Assert.False(g.ContainsVertex(-1)); Assert.False(g.ContainsVertex(1)); }
    [Fact] public void GetVertex_ThrowsForMissingId() { var g = new Graph<string>(); var e = Assert.Throws<VertexNotFoundException>(() => g.GetVertex(42)); Assert.Equal(42, e.VertexId); }
    [Fact] public void AddVertexAfterClear_RestartsIds() { var g = new Graph<int>(); g.AddVertex(1); g.Clear(); Assert.Equal(0, g.AddVertex(2)); }
    [Fact] public void Clear_ResetsCountsAndEmptyState() { var g = new Graph<int>(); int a = g.AddVertex(1), b = g.AddVertex(2); g.AddEdge(a, b); g.Clear(); Assert.True(g.IsEmpty); Assert.Equal(0, g.VertexCount); Assert.Equal(0, g.EdgeCount); }
    [Fact] public void Clear_OnEmptyGraphRemainsEmpty() { var g = new Graph<int>(); g.Clear(); Assert.True(g.IsEmpty); }
    [Fact] public void AddManyVertices_RemainsOrderedById() { var g = new Graph<int>(); for (int i = 0; i < 100; i++) g.AddVertex(i); var ids = new List<int>(); GraphAlgorithms.ForEach(g.GetConstVertexIterator(), v => ids.Add(v.Id)); Assert.Equal(Enumerable.Range(0, 100), ids); }
}
