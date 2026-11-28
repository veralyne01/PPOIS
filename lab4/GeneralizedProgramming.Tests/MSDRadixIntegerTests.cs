using GeneralizedProgramming;
using Xunit;

namespace GeneralizedProgramming.Tests;

public class MSDRadixIntegerTests
{
    [Fact] public void SortArray_HandlesEmptyAndSingleton() { int[] a = []; MSDRadixSort.Sort(a); Assert.Empty(a); int[] b = [5]; MSDRadixSort.Sort(b); Assert.Equal([5], b); }
    [Fact] public void SortArray_OrdersNegativeAndPositiveValues() { int[] a = [0, -1, 10, int.MinValue, int.MaxValue, -100, 1]; MSDRadixSort.Sort(a); Assert.Equal([int.MinValue, -100, -1, 0, 1, 10, int.MaxValue], a); }
    [Fact] public void SortArray_HandlesRepeatedValues() { int[] a = [2, 1, 2, -1, 1, -1]; MSDRadixSort.Sort(a); Assert.Equal([-1, -1, 1, 1, 2, 2], a); }
    [Fact] public void SortArray_HandlesCommonHighBytes() { int[] a = [256, 1, 255, 0, 65536, -256, -1]; MSDRadixSort.Sort(a); Assert.Equal([-256, -1, 0, 1, 255, 256, 65536], a); }
    [Fact] public void SortList_OrdersValues() { var a = new List<int> { 9, -3, 0, 4, -10 }; MSDRadixSort.Sort(a); Assert.Equal(new[] { -10, -3, 0, 4, 9 }, a); }
    [Fact] public void SortList_HandlesEmptyList() { var a = new List<int>(); MSDRadixSort.Sort(a); Assert.Empty(a); }
    [Fact] public void SortGenericArray_RejectsNullSelector() => Assert.Throws<ArgumentNullException>(() => MSDRadixSort.Sort(new[] { 1, 2 }, (Func<int, int>)null!));
    [Fact] public void SortGenericList_RejectsNullList() => Assert.Throws<ArgumentNullException>(() => MSDRadixSort.Sort<int>((List<int>)null!, x => x));
    [Fact] public void SortGenericList_RejectsNullSelector() => Assert.Throws<ArgumentNullException>(() => MSDRadixSort.Sort(new List<int> { 1 }, (Func<int, int>)null!));
    [Fact] public void Sort_UsesIntegerKeyForObjects() { var a = new[] { ("x", 3), ("y", -2), ("z", 3) }; MSDRadixSort.Sort(a, x => x.Item2); Assert.Equal(new[] { "y", "x", "z" }, a.Select(x => x.Item1)); }
    [Fact] public void Sort_IsStableForEqualIntegerKeys() { var a = new[] { ("first", 2), ("second", 1), ("third", 2), ("fourth", 1) }; MSDRadixSort.Sort(a, x => x.Item2); Assert.Equal(new[] { "second", "fourth", "first", "third" }, a.Select(x => x.Item1)); }
}
