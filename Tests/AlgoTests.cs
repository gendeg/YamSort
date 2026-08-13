using YamSort;

namespace AlgorithmTests;


public class BasicFunctionTests
{
    [Fact]
    public void AlreadySorted()
    {
        int[] referenceArray = [ 1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                                21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 
                                41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 
                                61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
                                81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100];

        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void ReverseSorted()
    {
        int[] referenceArray = [100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81,
                                80, 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61,
                                60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41,
                                40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21,
                                20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1];

        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void DuplicateValues()
    {
        int[] referenceArray = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void MixedSignNumbers()
    {
        int[] referenceArray = [1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10,
                                1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 1, -2, 3, -4, 5, -6, 7, -8, 9, -10,
                                1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10,
                                1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 1, -2, 3, -4, 5, -6, 7, -8, 9, -10,
                                1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10];

        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void AllSameValue()
    {
        int[] referenceArray = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];

        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void OddLength()
    {
        int[] referenceArray = [ 1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                                21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 
                                41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 
                                61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
                                81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101];

        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }
}


public class DoubleFunctionTests
{
    [Fact]
    public void AlreadySorted()
    {
        double[] referenceArray = [ 1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                                21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 
                                41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 
                                61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
                                81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100];

        double[] array = (double[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void ReverseSorted()
    {
        double[] referenceArray = [100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81,
                                80, 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61,
                                60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41,
                                40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21,
                                20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1];

        double[] array = (double[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void DuplicateValues()
    {
        double[] referenceArray = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        double[] array = (double[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void MixedSignNumbers()
    {
        double[] referenceArray = [1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10,
                                1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 1, -2, 3, -4, 5, -6, 7, -8, 9, -10,
                                1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10,
                                1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 1, -2, 3, -4, 5, -6, 7, -8, 9, -10,
                                1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10];

        double[] array = (double[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void AllSameValue()
    {
        double[] referenceArray = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];

        double[] array = (double[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void OddLength()
    {
        double[] referenceArray = [ 1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                                21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 
                                41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 
                                61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
                                81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101];

        double[] array = (double[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    // This tests the .NET convention of NaN being the first value rather than the IEEE 754 convention of NaN being the last value
    [Fact]
    public void SortLargeArrayWithNaN()
    {
        double[] array = [1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0, 29.0, 30.0, 31.0, 32.0, 33.0, 34.0, 35.0, 36.0, 37.0, 38.0, 39.0, 40.0, 41.0, 42.0, 43.0, 44.0, 45.0, 46.0, 47.0, 48.0, double.NaN, 50.0];
        YamSorter.Sort(array);

        double[] referenceArray = [double.NaN, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0, 29.0, 30.0, 31.0, 32.0, 33.0, 34.0, 35.0, 36.0, 37.0, 38.0, 39.0, 40.0, 41.0, 42.0, 43.0, 44.0, 45.0, 46.0, 47.0, 48.0, 50.0];
        Assert.Equal(referenceArray, array);
        
        array = [1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, double.NaN, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0, 29.0, 30.0, 31.0, 32.0, 33.0, double.NaN, 35.0, 36.0, 37.0, 38.0, 39.0, 40.0, 41.0, 42.0, 43.0, 44.0, 45.0, 46.0, 47.0, 48.0, double.NaN, 50.0];
        YamSorter.Sort(array);

        referenceArray = [double.NaN, double.NaN, double.NaN, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0, 29.0, 30.0, 31.0, 32.0, 33.0, 35.0, 36.0, 37.0, 38.0, 39.0, 40.0, 41.0, 42.0, 43.0, 44.0, 45.0, 46.0, 47.0, 48.0, 50.0];
        Assert.Equal(referenceArray, array);
    }
}


public class StringFunctionTests
{
    [Fact]
    public void AlreadySorted()
    {
        string[] referenceArray = new int[]
            {1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 
            61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
            81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100}
            .Select(x => x.ToString()).ToArray();

        string[] array = (string[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void ReverseSorted()
    {
        string[] referenceArray = new int[]
            {100, 99, 98, 97, 96, 95, 94, 93, 92, 91, 90, 89, 88, 87, 86, 85, 84, 83, 82, 81,
            80, 79, 78, 77, 76, 75, 74, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61,
            60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41,
            40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21,
            20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1}
            .Select(x => x.ToString()).ToArray();

        string[] array = (string[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void DuplicateValues()
    {
        string[] referenceArray = new int[]
            {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
             1, 2, 3, 4, 5, 6, 7, 8, 9, 10}
            .Select(x => x.ToString()).ToArray();

        string[] array = (string[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void MixedSignNumbers()
    {
        string[] referenceArray = new int[]
            {1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10,
             1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 1, -2, 3, -4, 5, -6, 7, -8, 9, -10,
             1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10,
             1, -2, 3, -4, 5, -6, 7, -8, 9, -10, 1, -2, 3, -4, 5, -6, 7, -8, 9, -10,
             1, -2, 3, -4, 5, -6, 7, -8, 9, -10, -1, 2, -3, 4, -5, 6, -7, 8, -9, 10}
            .Select(x => x.ToString()).ToArray();

        string[] array = (string[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void AllSameValue()
    {
        string[] referenceArray = new int[]
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
             1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
             1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
             1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
             1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
            .Select(x => x.ToString()).ToArray();

        string[] array = (string[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void OddLength()
    {
        string[] referenceArray = new int[]
            {1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 
            61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
            81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101}
            .Select(x => x.ToString()).ToArray();

        string[] array = (string[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void NullValues()
    {
        Random rand = new(DateTime.UtcNow.Millisecond);
        string[] referenceArray = new string[100];
        for (int j = 0; j < 49; j++)
        {
            referenceArray[j * 2] = rand.Next(0, 100).ToString();
        }

        string[] array = (string[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }
}