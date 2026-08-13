using YamSort;

namespace StabilityTests;


public struct StableInt(int sortValue, int refValue) : IComparable<StableInt>
{
    public int SortVal { get; set; } = sortValue;
    public int RefVal { get; set; } = refValue;

    public int CompareTo(StableInt other)
    {
        return SortVal.CompareTo(other.SortVal);
    }

    public override string ToString()
    {
        return $"({SortVal}, {RefVal})";
    }
}


public class BasicTests
{
    [Fact]
    public void SortStable()
    {
        StableInt[] referenceArray = [
            new StableInt(1, 0),
            new StableInt(2, 1),
            new StableInt(1, 2),
            new StableInt(2, 3),
            new StableInt(1, 4)
        ];
        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);

        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }
}


public class ArraySize100
{
    StableInt[] starterArray = Enumerable.Range(1, 100)
        .Select((x, i) => new StableInt(x, i))
        .ToArray();

    public static IEnumerable<object[]> GetNumberRange100
    {
        get {for (int i = 0; i <= 100; i++) yield return new object[] { i };}
    }

    public static IEnumerable<object[]> GetNumberRange90
    {
        get {for (int i = 0; i <= 90; i++) yield return new object[] { i };}
    }

    public static IEnumerable<object[]> GetNumberRange50
    {
        get {for (int i = 0; i <= 50; i++) yield return new object[] { i };}
    }


    [Fact]
    public void BoundaryValues()
    {
        StableInt[] referenceArray = (StableInt[])starterArray.Clone();
        referenceArray[0] = new StableInt(1, 0);
        referenceArray[24] = new StableInt(1, 1);
        referenceArray[25] = new StableInt(1, 2);
        referenceArray[49] = new StableInt(1, 3);
        referenceArray[50] = new StableInt(1, 4);
        referenceArray[74] = new StableInt(1, 5);
        referenceArray[75] = new StableInt(1, 6);
        referenceArray[99] = new StableInt(1, 7);

        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Fact]
    public void ReverseSequentialBoundaryValues()
    {
        StableInt[] referenceArray = Enumerable.Range(1, 100)
        .Select((x, i) => new StableInt(x, i))
        .Reverse().ToArray();
        referenceArray[0] = new StableInt(1, 0);
        referenceArray[24] = new StableInt(1, 1);
        referenceArray[25] = new StableInt(1, 2);
        referenceArray[49] = new StableInt(1, 3);
        referenceArray[50] = new StableInt(1, 4);
        referenceArray[74] = new StableInt(1, 5);
        referenceArray[75] = new StableInt(1, 6);
        referenceArray[99] = new StableInt(1, 7);

        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Fact]
    public void AllEqualStaysStable()
    {
        StableInt[] referenceArray = (StableInt[])starterArray.Clone();
        for (int i = 0; i < referenceArray.Length; i++)
        {
            referenceArray[i] = new StableInt(1, i);
        }

        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Theory]
    [MemberData(nameof(GetNumberRange90))]
    public void LongStreakAt(int startIndex)
    {
        StableInt[] referenceArray = (StableInt[])starterArray.Clone();
        referenceArray[startIndex] = new StableInt(1, 0);
        referenceArray[startIndex + 1] = new StableInt(1, 1);
        referenceArray[startIndex + 2] = new StableInt(1, 2);
        referenceArray[startIndex + 3] = new StableInt(1, 3);
        referenceArray[startIndex + 4] = new StableInt(1, 4);
        referenceArray[startIndex + 5] = new StableInt(1, 5);
        referenceArray[startIndex + 6] = new StableInt(1, 6);
        referenceArray[startIndex + 7] = new StableInt(1, 7);
        referenceArray[startIndex + 8] = new StableInt(1, 8);
        referenceArray[startIndex + 9] = new StableInt(1, 9);

        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Theory]
    [MemberData(nameof(GetNumberRange50))]
    public void SplitStreakAt(int startIndex)
    {
        StableInt[] referenceArray = (StableInt[])starterArray.Clone();
        referenceArray[startIndex] = new StableInt(1, 0);
        referenceArray[startIndex + 1] = new StableInt(1, 1);
        referenceArray[startIndex + 2] = new StableInt(1, 2);
        referenceArray[startIndex + 3] = new StableInt(1, 3);
        referenceArray[startIndex + 4] = new StableInt(1, 4);
        referenceArray[startIndex + 40] = new StableInt(1, 5);
        referenceArray[startIndex + 41] = new StableInt(1, 6);
        referenceArray[startIndex + 42] = new StableInt(1, 7);
        referenceArray[startIndex + 43] = new StableInt(1, 8);
        referenceArray[startIndex + 44] = new StableInt(1, 9);

        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Fact]
    public void HighValueOnLeft()
    {
        StableInt[] referenceArray = (StableInt[])starterArray.Clone();
        referenceArray[42] = new StableInt(98, 105);

        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Fact]
    public void LowValueOnRight()
    {
        StableInt[] referenceArray = (StableInt[])starterArray.Clone();
        referenceArray[97] = new StableInt(3, 105);

        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Fact]
    public void LeftEncountersValueWithRight()
    {
        StableInt[] referenceArray = (StableInt[])starterArray.Clone();
        referenceArray[40] = new StableInt(21, 0);
        referenceArray[41] = new StableInt(56, 1);
        referenceArray[42] = new StableInt(92, 2);
        referenceArray[43] = new StableInt(28, 3);
        referenceArray[44] = new StableInt(59, 4);
        referenceArray[50] = new StableInt(21, 5);
        referenceArray[51] = new StableInt(56, 6);
        referenceArray[52] = new StableInt(92, 7);
        referenceArray[53] = new StableInt(28, 8);
        referenceArray[54] = new StableInt(59, 9);

        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Fact]
    public void AscendingRunWithEqualElements()
    {
        StableInt[] referenceArray = Enumerable.Range(1, 100)
            .Select((x, i) => new StableInt(x/2, i))
            .ToArray();
        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }

    [Fact]
    public void DescendingRunWithEqualElements()
    {
        StableInt[] referenceArray = Enumerable.Range(1, 100).Reverse()
            .Select((x, i) => new StableInt(x/2, i))
            .ToArray();
        StableInt[] array = (StableInt[])referenceArray.Clone();
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
    }
}