using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphIteratorTests
{
    [Fact] public void VertexIterator_StartsBeforeFirstElement() { var g = Build(); var it = g.GetVertexIterator(); Assert.False(it.HasCurrent); Assert.Throws<InvalidIteratorException>(() => _ = it.Current); }
    [Fact] public void VertexIterator_MovesForwardAndStopsAtEnd() { var g = Build(); var it = g.GetConstVertexIterator(); Assert.True(it.MoveNext()); Assert.Equal(0, it.Current.Id); Assert.True(it.MoveNext()); Assert.Equal(1, it.Current.Id); Assert.True(it.MoveNext()); Assert.Equal(2, it.Current.Id); Assert.False(it.MoveNext()); Assert.False(it.MoveNext()); }
    [Fact] public void VertexIterator_MovesBackward() { var g = Build(); var it = g.GetConstVertexIterator(); while (it.MoveNext()) { } Assert.True(it.MovePrev()); Assert.Equal(2, it.Current.Id); Assert.True(it.MovePrev()); Assert.Equal(1, it.Current.Id); Assert.True(it.MovePrev()); Assert.Equal(0, it.Current.Id); Assert.False(it.MovePrev()); }
    [Fact] public void VertexIterator_ResetReturnsBeforeBeginning() { var g = Build(); var it = g.GetConstVertexIterator(); it.MoveNext(); it.MoveNext(); it.Reset(); Assert.False(it.HasCurrent); Assert.True(it.MoveNext()); Assert.Equal(0, it.Current.Id); }
    [Fact] public void EmptyIteratorHasNoCurrent() { var it = new Graph<int>().GetConstVertexIterator(); Assert.False(it.MoveNext()); Assert.False(it.MovePrev()); Assert.False(it.HasCurrent); Assert.Throws<InvalidIteratorException>(() => _ = it.Current); }
    [Fact] public void ReverseVertexIteratorUsesReverseOrder() { var g = Build(); Assert.Equal(new[] { 2, 1, 0 }, Read(g.GetConstReverseVertexIterator(), v => v.Id)); }
    [Fact] public void EdgeIteratorTraversesEdges() { var g = Build(); g.AddEdge(0, 2); g.AddEdge(1, 2); Assert.Equal(new[] { (0, 2), (1, 2) }, Read(g.GetConstEdgeIterator(), e => (e.From.Id, e.To.Id))); }
    [Fact] public void ReverseEdgeIteratorReversesSnapshot() { var g = Build(); g.AddEdge(0, 1); g.AddEdge(1, 2); Assert.Equal(new[] { (1, 2), (0, 1) }, Read(g.GetConstReverseEdgeIterator(), e => (e.From.Id, e.To.Id))); }
    [Fact] public void IteratorIsInvalidatedByAddingVertex() { var g = Build(); var it = g.GetConstVertexIterator(); g.AddVertex(3); Assert.False(it.IsValid); Assert.Throws<InvalidIteratorException>(() => it.MoveNext()); }
    [Fact] public void IteratorIsInvalidatedByAddingEdge() { var g = Build(); var it = g.GetConstVertexIterator(); g.AddEdge(0, 1); Assert.Throws<InvalidIteratorException>(() => it.Reset()); }
    [Fact] public void IteratorIsInvalidatedByRemovingEdge() { var g = Build(); g.AddEdge(0, 1); var it = g.GetConstEdgeIterator(); g.RemoveEdge(0, 1); Assert.Throws<InvalidIteratorException>(() => it.MovePrev()); }
    [Fact] public void IteratorIsInvalidatedByRemovingVertex() { var g = Build(); var it = g.GetConstVertexIterator(); g.RemoveVertex(0); Assert.Throws<InvalidIteratorException>(() => _ = it.Current); }
    [Fact] public void IteratorIsInvalidatedByCopyFrom() { var g = Build(); var it = g.GetConstVertexIterator(); g.CopyFrom(Build()); Assert.Throws<InvalidIteratorException>(() => it.MoveNext()); }
    [Fact] public void RemoveVertexByIterator_RemovesCurrentVertex() { var g = Build(); var it = g.GetVertexIterator(); it.MoveNext(); it.MoveNext(); g.RemoveVertex(it); Assert.False(g.ContainsVertex(1)); }
    [Fact] public void RemoveEdgeByIterator_RemovesCurrentEdge() { var g = Build(); g.AddEdge(0, 1); var it = g.GetEdgeIterator(); it.MoveNext(); g.RemoveEdge(it); Assert.Equal(0, g.EdgeCount); }
    [Fact] public void RemoveByIterator_RejectsIteratorFromAnotherGraph() { var a = Build(); var b = Build(); var it = b.GetVertexIterator(); it.MoveNext(); Assert.Throws<InvalidIteratorException>(() => a.RemoveVertex(it)); }
    [Fact] public void RemoveByIterator_RejectsNullIterator() { var g = Build(); Assert.Throws<ArgumentNullException>(() => g.RemoveVertex((GraphIterator<Vertex<int>>)null!)); Assert.Throws<ArgumentNullException>(() => g.RemoveEdge((GraphIterator<Edge<int>>)null!)); }
    [Fact] public void RemoveByIterator_RejectsIteratorWithoutCurrent() { var g = Build(); var it = g.GetVertexIterator(); Assert.Throws<InvalidIteratorException>(() => g.RemoveVertex(it)); }

    [Fact]
    public void AllMutableReverseAndConstIteratorFactories_ReturnUsableIterators()
    {
        var g = Build();
        g.AddEdge(0, 1);
        Assert.Equal(3, Read(g.GetReverseVertexIterator(), v => v.Id).Count);
        Assert.Equal(1, Read(g.GetReverseEdgeIterator(), e => e.From.Id).Count);
        Assert.Equal(1, Read(g.GetIncidentEdgeIterator(0), e => e.To.Id).Count);
        Assert.Equal(1, Read(g.GetReverseIncidentEdgeIterator(0), e => e.To.Id).Count);
        Assert.Equal(1, Read(g.GetAdjacentVertexIterator(0), v => v.Id).Count);
        Assert.Equal(1, Read(g.GetReverseAdjacentVertexIterator(0), v => v.Id).Count);
        Assert.Equal(3, Read(g.GetConstReverseVertexIterator(), v => v.Id).Count);
    }

    private static Graph<int> Build() { var g = new Graph<int>(); g.AddVertex(10); g.AddVertex(20); g.AddVertex(30); return g; }
    private static List<TResult> Read<TItem, TResult>(GraphIteratorBase<TItem> it, Func<TItem, TResult> map) { var result = new List<TResult>(); GraphAlgorithms.ForEach(it, x => result.Add(map(x))); return result; }
}
