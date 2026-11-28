using Xunit;

namespace GeneralizedProgramming.Tests;

public class BinaryTreeSortArrayTests
{
    [Fact] public void Sort_EmptyArray_DoesNothing() { int[] a = []; BinaryTreeSort.Sort(a); Assert.Empty(a); }
    [Fact] public void Sort_SingleElement_DoesNothing() { int[] a = [7]; BinaryTreeSort.Sort(a); Assert.Equal([7], a); }
    [Fact] public void Sort_SortsAscending() { int[] a = [5, 1, 4, 2, 3]; BinaryTreeSort.Sort(a); Assert.Equal([1, 2, 3, 4, 5], a); }
    [Fact] public void Sort_HandlesDuplicates() { int[] a = [3, 1, 3, 2, 1]; BinaryTreeSort.Sort(a); Assert.Equal([1, 1, 2, 3, 3], a); }
    [Fact] public void Sort_HandlesAlreadySortedInput() { int[] a = [1, 2, 3, 4]; BinaryTreeSort.Sort(a); Assert.Equal([1, 2, 3, 4], a); }
    [Fact] public void Sort_HandlesReverseSortedInput() { int[] a = [4, 3, 2, 1]; BinaryTreeSort.Sort(a); Assert.Equal([1, 2, 3, 4], a); }
    [Fact] public void Sort_HandlesNegativeAndExtremeIntegers() { int[] a = [int.MaxValue, 0, int.MinValue, -1, 1]; BinaryTreeSort.Sort(a); Assert.Equal([int.MinValue, -1, 0, 1, int.MaxValue], a); }
    [Fact] public void Sort_UsesCustomComparer() { string[] a = ["a", "CCC", "bb"]; BinaryTreeSort.Sort(a, Comparer<string>.Create((x, y) => y.Length.CompareTo(x.Length))); Assert.Equal(["CCC", "bb", "a"], a); }
    [Fact] public void Sort_RejectsNullComparer() => Assert.Throws<ArgumentNullException>(() => BinaryTreeSort.Sort(new[] { 2, 1 }, null!));
    [Fact] public void Sort_WorksWithStudents() { var a = new[] { new Student("B", 20), new Student("A", 20), new Student("C", 18) }; BinaryTreeSort.Sort(a); Assert.Equal(new[] { "C (18)", "A (20)", "B (20)" }, a.Select(x => x.ToString())); }
}
