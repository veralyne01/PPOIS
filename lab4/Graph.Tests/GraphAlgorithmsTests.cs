using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphAlgorithmsTests
{
    [Fact] public void ForEach_VisitsEveryElementInOrder() { var g=new Graph<int>(); g.AddVertex(4); g.AddVertex(5); g.AddVertex(6); var seen=new List<int>(); GraphAlgorithms.ForEach(g.GetConstVertexIterator(),v=>seen.Add(v.Value)); Assert.Equal(new[]{4,5,6},seen); }
    [Fact] public void ForEach_EmptyIteratorDoesNothing() { var seen=new List<int>(); GraphAlgorithms.ForEach(new Graph<int>().GetConstVertexIterator(),v=>seen.Add(v.Value)); Assert.Empty(seen); }
    [Fact] public void ForEach_RejectsNullIterator() => Assert.Throws<ArgumentNullException>(()=>GraphAlgorithms.ForEach<int>(null!,_=>{}));
    [Fact] public void ForEach_RejectsNullAction() { var it=new Graph<int>().GetConstVertexIterator(); Assert.Throws<ArgumentNullException>(()=>GraphAlgorithms.ForEach(it,null!)); }
    [Fact] public void ForEach_PropagatesActionException() { var g=new Graph<int>(); g.AddVertex(1); Assert.Throws<InvalidOperationException>(()=>GraphAlgorithms.ForEach(g.GetConstVertexIterator(),_=>throw new InvalidOperationException())); }
    [Fact] public void ForEach_StopsWhenIteratorInvalidatedInsideCallback() { var g=new Graph<int>(); g.AddVertex(1); g.AddVertex(2); var it=g.GetConstVertexIterator(); Assert.Throws<InvalidIteratorException>(()=>GraphAlgorithms.ForEach(it,_=>g.AddVertex(3))); }
}
