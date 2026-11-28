using Xunit;

namespace GeneralizedProgramming.Tests;

public class BinaryTreeSortListTests
{
    [Fact] public void Sort_EmptyList_DoesNothing() { var x = new List<int>(); BinaryTreeSort.Sort(x); Assert.Empty(x); }
    [Fact] public void Sort_SingleItem_DoesNothing() { var x = new List<int> { 9 }; BinaryTreeSort.Sort(x); Assert.Equal(new[] { 9 }, x); }
    [Fact] public void Sort_SortsValues() { var x = new List<int> { 8, 3, 7, 1, 5 }; BinaryTreeSort.Sort(x); Assert.Equal(new[] { 1, 3, 5, 7, 8 }, x); }
    [Fact] public void Sort_PreservesDuplicates() { var x = new List<int> { 2, 2, 1, 2, 1 }; BinaryTreeSort.Sort(x); Assert.Equal(new[] { 1, 1, 2, 2, 2 }, x); }
    [Fact] public void Sort_UsesCustomComparer() { var x = new List<string> { "pear", "fig", "banana" }; BinaryTreeSort.Sort(x, StringComparer.Ordinal); Assert.Equal(new[] { "banana", "fig", "pear" }, x); }
    [Fact] public void Sort_RejectsNullList() => Assert.Throws<ArgumentNullException>(() => BinaryTreeSort.Sort((List<int>)null!));
    [Fact] public void Sort_RejectsNullComparer() => Assert.Throws<ArgumentNullException>(() => BinaryTreeSort.Sort(new List<int> { 2, 1 }, null!));
    [Fact] public void Sort_HandlesAllEqualValues() { var x = Enumerable.Repeat(4, 30).ToList(); BinaryTreeSort.Sort(x); Assert.All(x, v => Assert.Equal(4, v)); }
}
