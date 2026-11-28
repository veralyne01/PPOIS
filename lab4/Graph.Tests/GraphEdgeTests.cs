using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphEdgeTests
{
    [Fact] public void AddEdge_IncrementsEdgeCount() { var g=Make(out int a,out int b); g.AddEdge(a,b); Assert.Equal(1,g.EdgeCount); Assert.True(g.ContainsEdge(a,b)); }
    [Fact] public void AddEdge_AllowsSelfLoop() { var g=Make(out int a,out _); g.AddEdge(a,a); Assert.True(g.ContainsEdge(a,a)); Assert.Equal(1,g.EdgeCount); }
    [Fact] public void AddEdge_StoresOutgoingEdgesInSortedOrder() { var g=Make(out int a,out int b,out int c,out int d); g.AddEdge(a,d); g.AddEdge(a,b); g.AddEdge(a,c); var adjacent=new List<int>(); GraphAlgorithms.ForEach(g.GetConstAdjacentVertexIterator(a),v=>adjacent.Add(v.Id)); Assert.Equal(new[]{b,c,d},adjacent); }
    [Fact] public void AddEdge_RejectsDuplicate() { var g=Make(out int a,out int b); g.AddEdge(a,b); var e=Assert.Throws<EdgeAlreadyExistsException>(()=>g.AddEdge(a,b)); Assert.Equal(a,e.From); Assert.Equal(b,e.To); }
    [Fact] public void AddEdge_RejectsMissingSource() { var g=Make(out _,out int b); Assert.Throws<VertexNotFoundException>(()=>g.AddEdge(99,b)); }
    [Fact] public void AddEdge_RejectsMissingTarget() { var g=Make(out int a,out _); Assert.Throws<VertexNotFoundException>(()=>g.AddEdge(a,99)); }
    [Fact] public void ContainsEdge_ReturnsFalseForMissingSourceOrTarget() { var g=Make(out int a,out int b); Assert.False(g.ContainsEdge(99,b)); Assert.False(g.ContainsEdge(a,99)); }
    [Fact] public void RemoveEdge_DecrementsCount() { var g=Make(out int a,out int b); g.AddEdge(a,b); g.RemoveEdge(a,b); Assert.Equal(0,g.EdgeCount); Assert.False(g.ContainsEdge(a,b)); }
    [Fact] public void RemoveEdge_RejectsMissingEdge() { var g=Make(out int a,out int b); var e=Assert.Throws<EdgeNotFoundException>(()=>g.RemoveEdge(a,b)); Assert.Equal(a,e.From); Assert.Equal(b,e.To); }
    [Fact] public void RemoveEdge_RejectsMissingVertices() { var g=Make(out int a,out _); Assert.Throws<VertexNotFoundException>(()=>g.RemoveEdge(a,99)); Assert.Throws<VertexNotFoundException>(()=>g.RemoveEdge(99,a)); }
    [Fact] public void ReverseDirectionIsDifferentEdge() { var g=Make(out int a,out int b); g.AddEdge(a,b); Assert.False(g.ContainsEdge(b,a)); }
    private static Graph<string> Make(out int a,out int b) { var g=new Graph<string>(); a=g.AddVertex("A"); b=g.AddVertex("B"); return g; }
    private static Graph<string> Make(out int a,out int b,out int c,out int d) { var g=Make(out a,out b); c=g.AddVertex("C"); d=g.AddVertex("D"); return g; }
}
