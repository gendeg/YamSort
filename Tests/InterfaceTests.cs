using YamSort;

namespace InterfaceTests;

// These tests check interaction with the class itself
// Most of them will be caught by the "front gate" insertion sort and don't test the merge algorithm

public class IntInterfaceTests
{
    [Fact]
    public void SortEmpty()
    {
        int[] array = [];
        YamSorter.Sort(array);
        Assert.Empty(array);
    }

    [Fact]
    public void SortSingleInt()
    {
        int[] array = [1];
        YamSorter.Sort(array);

        int[] referenceArray = [1];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortTwoIntInOrder()
    {
        int[] array = [1, 2];
        YamSorter.Sort(array);

        int[] referenceArray = [1, 2];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortTwoIntNotInOrder()
    {
        int[] array = [2, 1];
        YamSorter.Sort(array);

        int[] referenceArray = [1, 2];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortThreeIntNotInOrder()
    {
        int[] array = [3, 1, 2];
        YamSorter.Sort(array);

        int[] referenceArray = [1, 2, 3];
        Assert.Equal(referenceArray, array);

        array = [3, 2, 1];
        YamSorter.Sort(array);
        Assert.Equal(referenceArray, array);

        array = [2, 1, 3];
        YamSorter.Sort(array);
        Assert.Equal(referenceArray, array);
    }
}


public class StringInterfaceTests
{
    [Fact]
    public void SortSingleString()
    {
        string[] array = ["a"];
        YamSorter.Sort(array);

        string[] referenceArray = ["a"];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortTwoStringInOrder()
    {
        string[] array = ["a", "b"];
        YamSorter.Sort(array);

        string[] referenceArray = ["a", "b"];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortTwoStringNotInOrder()
    {
        string[] array = ["b", "a"];
        YamSorter.Sort(array);

        string[] referenceArray = ["a", "b"];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortThreeStringNotInOrder()
    {
        string[] array = ["c", "a", "b"];
        YamSorter.Sort(array);

        string[] referenceArray = ["a", "b", "c"];
        Assert.Equal(referenceArray, array);

        array = ["c", "b", "a"];
        YamSorter.Sort(array);
        Assert.Equal(referenceArray, array);

        array = ["b", "a", "c"];
        YamSorter.Sort(array);
        Assert.Equal(referenceArray, array);
    }
}


public class DoubleInterfaceTests
{
    [Fact]
    public void SortSingleDouble()
    {
        double[] array = [1.0];
        YamSorter.Sort(array);

        double[] referenceArray = [1.0];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortTwoDoubleInOrder()
    {
        double[] array = [1.0, 2.0];
        YamSorter.Sort(array);

        double[] referenceArray = [1.0, 2.0];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortTwoDoubleNotInOrder()
    {
        double[] array = [2.0, 1.0];
        YamSorter.Sort(array);

        double[] referenceArray = [1.0, 2.0];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortThreeDoubleNotInOrder()
    {
        double[] array = [3.0, 1.0, 2.0];
        YamSorter.Sort(array);

        double[] referenceArray = [1.0, 2.0, 3.0];
        Assert.Equal(referenceArray, array);

        array = [3.0, 2.0, 1.0];
        YamSorter.Sort(array);
        Assert.Equal(referenceArray, array);

        array = [2.0, 1.0, 3.0];
        YamSorter.Sort(array);
        Assert.Equal(referenceArray, array);
    }

    // This tests the .NET convention of NaN being the first value rather than the IEEE 754 convention of NaN being the last value
    [Fact]
    public void SortDoubleWithNaN()
    {
        double[] array = [1.0, double.NaN, 2.0];
        YamSorter.Sort(array);

        double[] referenceArray = [double.NaN, 1.0, 2.0];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortDoubleWithInfinity()
    {
        double[] array = [double.PositiveInfinity, 1.0, 2.0];
        YamSorter.Sort(array);

        double[] referenceArray = [1.0, 2.0, double.PositiveInfinity];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortDoubleWithNegativeInfinity()
    {
        double[] array = [1.0, double.NegativeInfinity, 2.0];
        YamSorter.Sort(array);

        double[] referenceArray = [double.NegativeInfinity, 1.0, 2.0];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortDoubleWithNegativeZero()
    {
        double[] array = [-0.0, 1.0, 2.0];
        YamSorter.Sort(array);

        double[] referenceArray = [-0.0, 1.0, 2.0];
        Assert.Equal(referenceArray, array);
    }

    // This tests the .NET convention of NaN being the first value rather than the IEEE 754 convention of NaN being the last value
    [Fact]
    public void SortDoubleWithAllSpecialValues()
    {
        double[] array = [double.PositiveInfinity, double.NegativeInfinity, -0.0, -1.0, 0.0, 1.0, 2.0, double.NaN];
        YamSorter.Sort(array);

        double[] referenceArray = [double.NaN, double.NegativeInfinity, -1.0, -0.0, 0.0, 1.0, 2.0, double.PositiveInfinity];
        Assert.Equal(referenceArray, array);
    }
}


public class ListInterfaceTests
{
    [Fact]
    public void SortListOfInt()
    {
        List<int> list = [3, 1, 2];
        YamSorter.Sort(list);

        List<int> referenceList = [1, 2, 3];
        Assert.Equal(referenceList, list);
    }

    [Fact]
    public void SortListOfString()
    {
        List<string> list = ["c", "a", "b"];
        YamSorter.Sort(list);

        List<string> referenceList = ["a", "b", "c"];
        Assert.Equal(referenceList, list);
    }

    [Fact]
    public void SortListOfDouble()
    {
        List<double> list = [3.0, 1.0, 2.0];
        YamSorter.Sort(list);

        List<double> referenceList = [1.0, 2.0, 3.0];
        Assert.Equal(referenceList, list);
    }
}


public class CustomClassInterfaceTests
{
    public class Person(string name, int age) : IComparable<Person>
    {
        public string Name = name;
        public int Age = age;

        public int CompareTo(Person? other)
        {
            return Age.CompareTo(other?.Age ?? -1);
        }
    }

    [Fact]
    public void SortListOfCustomClass()
    {
        List<Person> list =
        [
            new Person("Alice", 30),
            new Person("Bob", 25),
            new Person("Charlie", 35)
        ];

        YamSorter.Sort(list);

        List<Person> referenceList =
        [
            new Person("Bob", 25),
            new Person("Alice", 30),
            new Person("Charlie", 35)
        ];

        Assert.Equal(referenceList.Select(p => p.Age), list.Select(p => p.Age));
    }
}


public class ReturningSortTests
{
    [Fact]
    public void SortReturningArray()
    {
        int[] array = [3, 1, 2];
        int[] sortedArray = YamSorter.SortReturning(array);

        int[] referenceArray = [1, 2, 3];
        Assert.Equal(referenceArray, sortedArray);
        Assert.Equal([3, 1, 2], array);
    }

    [Fact]
    public void SortReturningList()
    {
        List<int> list = [3, 1, 2];
        List<int> sortedList = YamSorter.SortReturning(list);

        List<int> referenceList = [1, 2, 3];
        Assert.Equal(referenceList, sortedList);
        Assert.Equal([3, 1, 2], list);
    }
}


public class CustomComparerTests
{
    public class StrangeComparer : Comparer<int>
    {
        public override int Compare(int x, int y)
        {
            if (x <= 5) x *= -1;
            if (y <= 5) y *= -1;
            return x.CompareTo(y);
        }
    }

    [Fact]
    public void SortArrayWithCustomComparer()
    {
        int[] array = [3, 1, 6, 5, 4, -1, -3, -6];
        YamSorter.Sort(array, new StrangeComparer());

        int[] referenceArray = [5, 4, 3, 1, -1, -3, 6, -6];
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SortListWithCustomComparer()
    {
        List<int> list = [3, 1, 6, 5, 4, -1, -3, -6];
        YamSorter.Sort(list, new StrangeComparer());

        List<int> referenceList = [5, 4, 3, 1, -1, -3, 6, -6];
        Assert.Equal(referenceList, list);
    }

    [Fact]
    public void SortReturningArrayWithCustomComparer()
    {
        int[] array = [3, 1, 6, 5, 4, -1, -3, -6];
        int[] sortedArray = YamSorter.SortReturning(array, new StrangeComparer());

        int[] referenceArray = [5, 4, 3, 1, -1, -3, 6, -6];
        Assert.Equal(referenceArray, sortedArray);
        Assert.Equal([3, 1, 6, 5, 4, -1, -3, -6], array);
    }

    [Fact]
    public void SortReturningListWithCustomComparer()
    {
        List<int> list = [3, 1, 6, 5, 4, -1, -3, -6];
        List<int> sortedList = YamSorter.SortReturning(list, new StrangeComparer());

        List<int> referenceList = [5, 4, 3, 1, -1, -3, 6, -6];
        Assert.Equal(referenceList, sortedList);
        Assert.Equal([3, 1, 6, 5, 4, -1, -3, -6], list);
    }
}