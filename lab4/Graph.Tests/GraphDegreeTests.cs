using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphDegreeTests
{
    [Fact] public void OutDegree_CountsOutgoingEdges() { var g = Create(out int a, out int b, out int c); g.AddEdge(a, b); g.AddEdge(a, c); Assert.Equal(2, g.OutDegree(a)); }
    [Fact] public void InDegree_CountsIncomingEdges() { var g = Create(out int a, out int b, out int c); g.AddEdge(a, c); g.AddEdge(b, c); Assert.Equal(2, g.InDegree(c)); }
    [Fact] public void Degree_IsSumOfInAndOut() { var g = Create(out int a, out int b, out int c); g.AddEdge(a, b); g.AddEdge(c, a); g.AddEdge(a, c); Assert.Equal(3, g.Degree(a)); }
    [Fact] public void Degrees_CountSelfLoopAsIncomingAndOutgoing() { var g = Create(out int a, out _, out _); g.AddEdge(a, a); Assert.Equal(1, g.InDegree(a)); Assert.Equal(1, g.OutDegree(a)); Assert.Equal(2, g.Degree(a)); }
    [Fact] public void DegreeMethods_ThrowForMissingVertex() { var g = new Graph<int>(); Assert.Throws<VertexNotFoundException>(() => g.InDegree(1)); Assert.Throws<VertexNotFoundException>(() => g.OutDegree(1)); Assert.Throws<VertexNotFoundException>(() => g.Degree(1)); }
    [Fact] public void EdgeDegree_CountsDistinctOtherIncidentEdgesAtBothEndpoints() { var g = Create(out int a, out int b, out int c, out int d); g.AddEdge(a, b); g.AddEdge(a, c); g.AddEdge(d, a); g.AddEdge(b, c); g.AddEdge(d, b); Assert.Equal(4, g.EdgeDegree(a, b)); }
    [Fact] public void EdgeDegree_HandlesSelfLoop() { var g = Create(out int a, out int b, out _); g.AddEdge(a, a); g.AddEdge(a, b); Assert.Equal(1, g.EdgeDegree(a, a)); }
    [Fact] public void EdgeDegree_ThrowsWhenEndpointMissing() { var g = Create(out int a, out _, out _); Assert.Throws<VertexNotFoundException>(() => g.EdgeDegree(a, 99)); }
    [Fact] public void EdgeDegree_ThrowsWhenEdgeDoesNotExist() { var g = Create(out int a, out int b, out _); var e = Assert.Throws<EdgeNotFoundException>(() => g.EdgeDegree(a, b)); Assert.Equal(a, e.From); Assert.Equal(b, e.To); }
    private static Graph<int> Create(out int a, out int b, out int c) { var g = new Graph<int>(); a = g.AddVertex(0); b = g.AddVertex(1); c = g.AddVertex(2); return g; }
    private static Graph<int> Create(out int a, out int b, out int c, out int d) { var g = Create(out a, out b, out c); d = g.AddVertex(3); return g; }
}
