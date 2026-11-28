using GeneralizedProgramming;
using Xunit;

namespace GeneralizedProgramming.Tests;

public class MSDRadixStringTests
{
    [Fact] public void SortArray_OrdersOrdinalStrings() { string[] a=["pear","app","apple",""]; MSDRadixSort.Sort(a); Assert.Equal(["","app","apple","pear"],a); }
    [Fact] public void SortArray_HandlesPrefixStrings() { string[] a=["abc","a","ab","abcd",""]; MSDRadixSort.Sort(a); Assert.Equal(["","a","ab","abc","abcd"],a); }
    [Fact] public void SortArray_HandlesUnicode() { string[] a=["я","a","Ё","😀","中",""]; var expected=a.OrderBy(x=>x,StringComparer.Ordinal).ToArray(); MSDRadixSort.Sort(a); Assert.Equal(expected,a); }
    [Fact] public void SortArray_HandlesDuplicates() { string[] a=["b","a","b","a","a"]; MSDRadixSort.Sort(a); Assert.Equal(["a","a","a","b","b"],a); }
    [Fact] public void SortList_OrdersStrings() { var a=new List<string>{"z","A","a","B"}; MSDRadixSort.Sort(a); Assert.Equal(a.OrderBy(x=>x,StringComparer.Ordinal),a); }
    [Fact] public void SortGenericArray_RejectsNullKey() => Assert.Throws<ArgumentException>(()=>MSDRadixSort.Sort(new[]{"a","b"},x=>(string)null!));
    [Fact] public void SortGenericList_RejectsNullKey() => Assert.Throws<ArgumentException>(()=>MSDRadixSort.Sort(new List<string>{"a","b"},x=>(string)null!));
    //[Fact] public void SortGenericArray_RejectsNullArray() => Assert.Throws<ArgumentNullException>(()=>MSDRadixSort.Sort<string>(null!,x=>x));
    [Fact] public void SortGenericArray_RejectsNullSelector() => Assert.Throws<ArgumentNullException>(()=>MSDRadixSort.Sort(new[]{"a"},(Func<string,string>)null!));
    [Fact] public void SortGenericList_RejectsNullList() => Assert.Throws<ArgumentNullException>(()=>MSDRadixSort.Sort<string>((List<string>)null!,x=>x));
    [Fact] public void SortGenericList_RejectsNullSelector() => Assert.Throws<ArgumentNullException>(()=>MSDRadixSort.Sort(new List<string>{"a"},(Func<string,string>)null!));
    [Fact] public void Sort_IsStableForEqualStringKeys() { var a=new[]{("one","x"),("two","a"),("three","x")}; MSDRadixSort.Sort(a,x=>x.Item2); Assert.Equal(new[]{"two","one","three"},a.Select(x=>x.Item1)); }
}
