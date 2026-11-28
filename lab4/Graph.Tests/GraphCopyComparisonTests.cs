using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphCopyComparisonTests
{
    [Fact] public void CopyConstructor_CopiesVerticesAndEdges() { var g = Build(); var copy = new Graph<string>(g); Assert.Equal(g, copy); Assert.Equal(2, copy.EdgeCount); }
    [Fact] public void CopyConstructor_RejectsNull() => Assert.Throws<ArgumentNullException>(() => new Graph<string>(null!));
    [Fact] public void Clone_CreatesIndependentGraph() { var g = Build(); var clone = g.Clone(); clone.AddVertex("extra"); Assert.Equal(3, g.VertexCount); Assert.Equal(4, clone.VertexCount); }
    [Fact] public void CopyFrom_ReplacesCurrentContent() { var source = Build(); var target = new Graph<string>(); target.AddVertex("old"); target.CopyFrom(source); Assert.Equal(source, target); }
    [Fact] public void CopyFrom_SelfIsNoOp() { var g = Build(); var it = g.GetVertexIterator(); g.CopyFrom(g); Assert.True(it.IsValid); Assert.Equal(3, g.VertexCount); }
    [Fact] public void CopyFrom_RejectsNull() { var g = new Graph<string>(); Assert.Throws<ArgumentNullException>(() => g.CopyFrom(null!)); }
    [Fact] public void CopyFrom_InvalidatesExistingIterator() { var a = Build(); var b = Build(); var it = a.GetVertexIterator(); a.CopyFrom(b); Assert.False(it.IsValid); }
    [Fact] public void Equals_ReturnsTrueForSameGraph() { var g = Build(); Assert.True(g.Equals(g)); Assert.True(g.Equals((object)g)); }
    [Fact] public void Equals_ReturnsFalseForNullOrDifferentType() { var g = Build(); Assert.False(g.Equals((Graph<string>?)null)); Assert.False(g.Equals("graph")); }
    [Fact] public void Equals_DetectsDifferentVertexCount() { Assert.NotEqual(new Graph<string>(), Build()); }
    [Fact] public void Equals_DetectsDifferentEdgeCount() { var a = Build(); var b = Build(); b.RemoveEdge(0, 1); Assert.NotEqual(a, b); }
    [Fact] public void Equals_DetectsDifferentVertexValue() { var a = Build(); var b = new Graph<string>(); b.AddVertex("X"); b.AddVertex("B"); b.AddVertex("C"); b.AddEdge(0, 1); b.AddEdge(1, 2); Assert.NotEqual(a, b); }
    [Fact]
    public void Equals_DetectsDifferentEdgesWithSameEdgeCount()
    {
        var a = Three(); var b = Three();
        a.AddEdge(0, 1); a.AddEdge(1, 2);
        b.AddEdge(0, 2); b.AddEdge(1, 0);
        Assert.NotEqual(a, b);
    }
    [Fact] public void CompareTo_NullReturnsPositive() => Assert.True(Build().CompareTo(null) > 0);
    [Fact] public void CompareTo_SameReferenceReturnsZero() { var g = Build(); Assert.Equal(0, g.CompareTo(g)); }
    [Fact] public void CompareTo_OrdersByVertexCountFirst() { var a = new Graph<string>(); var b = Build(); Assert.True(a.CompareTo(b) < 0); }
    [Fact] public void CompareTo_OrdersByEdgeCountSecond() { var a = new Graph<string>(); var b = new Graph<string>(); a.AddVertex("A"); a.AddVertex("B"); b.AddVertex("A"); b.AddVertex("B"); b.AddEdge(0, 1); Assert.True(a.CompareTo(b) < 0); }
    [Fact] public void CompareTo_OrdersByVertexValue() { var a = new Graph<string>(); a.AddVertex("A"); var b = new Graph<string>(); b.AddVertex("B"); Assert.True(a.CompareTo(b) < 0); }
    [Fact]
    public void CompareTo_UsesOutgoingListLengthAfterCommonTargets()
    {
        var a = Three(); var b = Three();
        a.AddEdge(0, 1); a.AddEdge(0, 2);
        b.AddEdge(0, 1); b.AddEdge(1, 2);
        Assert.True(a.CompareTo(b) > 0);
    }
    [Fact]
    public void CompareOperators_HandleTwoNullReferences()
    {
        Graph<string>? left = null; Graph<string>? right = null;
        Assert.True(left == right); Assert.False(left != right);
        Assert.True(left <= right); Assert.True(left >= right);
        Assert.False(left < right); Assert.False(left > right);
    }
    [Fact] public void CompareTo_OrdersByOutgoingTargets() { var a = Three(); var b = Three(); a.AddEdge(0, 1); b.AddEdge(0, 2); Assert.True(a.CompareTo(b) < 0); }
    [Fact] public void Operators_HandleNulls() { Graph<string>? nil = null; var g = new Graph<string>(); Assert.True(nil == null); Assert.True(nil != g); Assert.True(nil < g); Assert.True(nil <= g); Assert.True(g > null); Assert.True(g >= null); }
    [Fact] public void Operators_CompareEqualAndDifferentGraphs() { var a = Build(); var b = Build(); Assert.True(a == b); Assert.False(a != b); Assert.True(a <= b); Assert.True(a >= b); b.AddVertex("D"); Assert.True(a < b); Assert.True(b > a); }
    [Fact] public void GetHashCode_IsConsistentForEqualGraphs() { var a = Build(); var b = Build(); Assert.Equal(a.GetHashCode(), b.GetHashCode()); }
    [Fact] public void ToString_FormatsEmptyGraph() { var g = new Graph<string>(); Assert.Contains("Вершины (0): -", g.ToString()); Assert.Contains("Рёбра (0): -", g.ToString()); }
    [Fact] public void ToString_FormatsVerticesAndEdges() { var g = Build(); var s = g.ToString(); Assert.Contains("Вершины (3):", s); Assert.Contains("Рёбра (2):", s); Assert.Contains("0(A) -> 1(B)", s); }
    private static Graph<string> Build() { var g = Three(); g.AddEdge(0, 1); g.AddEdge(1, 2); return g; }
    private static Graph<string> Three() { var g = new Graph<string>(); g.AddVertex("A"); g.AddVertex("B"); g.AddVertex("C"); return g; }
}
