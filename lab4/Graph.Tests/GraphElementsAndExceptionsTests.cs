using GeneralizedProgramming;
using Xunit;

namespace Graph.Tests;

public class GraphElementsAndExceptionsTests
{
    [Fact] public void VertexRecord_StoresComponents() { var v = new Vertex<string>(7, "X"); Assert.Equal(7, v.Id); Assert.Equal("X", v.Value); }
    [Fact] public void VertexRecord_ToStringFormatsValue() => Assert.Equal("7(X)", new Vertex<string>(7, "X").ToString());
    [Fact] public void VertexRecord_UsesValueEquality() { Assert.Equal(new Vertex<int>(1, 2), new Vertex<int>(1, 2)); Assert.NotEqual(new Vertex<int>(1, 2), new Vertex<int>(1, 3)); }
    [Fact] public void EdgeRecord_StoresEndpoints() { var a = new Vertex<string>(0, "A"); var b = new Vertex<string>(1, "B"); var e = new Edge<string>(a, b); Assert.Equal(a, e.From); Assert.Equal(b, e.To); }
    [Fact] public void EdgeRecord_ToStringFormatsDirectedEdge() { var e = new Edge<int>(new Vertex<int>(1, 10), new Vertex<int>(2, 20)); Assert.Equal("1(10) -> 2(20)", e.ToString()); }
    [Fact] public void EdgeRecord_UsesValueEquality() { var a = new Edge<int>(new Vertex<int>(0, 1), new Vertex<int>(1, 2)); var b = new Edge<int>(new Vertex<int>(0, 1), new Vertex<int>(1, 2)); Assert.Equal(a, b); }
    [Fact] public void GraphException_PreservesMessage() { var e = new GraphException("custom"); Assert.Equal("custom", e.Message); }
    [Fact] public void InvalidIteratorException_IsGraphException() { var e = new InvalidIteratorException("invalid"); Assert.IsAssignableFrom<GraphException>(e); }
    [Fact] public void VertexNotFoundException_ContainsId() { var e = new VertexNotFoundException(15); Assert.Equal(15, e.VertexId); Assert.Contains("15", e.Message); }
    [Fact] public void EdgeNotFoundException_ContainsEndpoints() { var e = new EdgeNotFoundException(2, 4); Assert.Equal(2, e.From); Assert.Equal(4, e.To); }
    [Fact] public void EdgeAlreadyExistsException_ContainsEndpoints() { var e = new EdgeAlreadyExistsException(3, 8); Assert.Equal(3, e.From); Assert.Equal(8, e.To); }
}
