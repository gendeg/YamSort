using StabilityTests;
using InterfaceTests;
using YamSort;

namespace FuzzingTests;


public class InputShapes
{
    Random rand = new(DateTime.UtcNow.Millisecond);
    const int ARRAY_SIZE = 500000;
    const int LOOP_COUNT = 10;

    [Fact]
    public void Sequential()
    {
        int[] referenceArray = Enumerable.Range(1, ARRAY_SIZE).ToArray();
        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void ReverseSequential()
    {
        int[] referenceArray = Enumerable.Range(1, ARRAY_SIZE).Reverse().ToArray();
        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void Random()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            int[] referenceArray = Enumerable.Range(1, ARRAY_SIZE).OrderBy(_ => rand.Next()).ToArray();
            int[] array = (int[])referenceArray.Clone();
            YamSorter.Sort(array);
            Array.Sort(referenceArray);
            Assert.Equal(referenceArray, array);
        }
    }

    [Fact]
    public void ClusteredRandom()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            double mean = 1000;
            double stdDev = Math.Max(ARRAY_SIZE / 200, 100);

            int[] referenceArray = new int[ARRAY_SIZE];
            for (int j = 0; j < ARRAY_SIZE; j++) {
                double u1 = 1.0 - rand.NextDouble(); // uniform(0,1]
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                
                referenceArray[j] = (int)(mean + stdDev * randStdNormal);
            }

            int[] array = (int[])referenceArray.Clone();
            YamSorter.Sort(array);
            Array.Sort(referenceArray);
            Assert.Equal(referenceArray, array);
        }
    }

    [Fact]
    public void SplitClusteredRandom()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            double lowMean = 1000;
            double highMean = 100_000_000;
            double stdDev = 300;
            int thisN = ARRAY_SIZE/2;

            int[] referenceArray = new int[thisN];
            for (int j = 0; j < thisN; j++) {
                double u1 = 1.0 - rand.NextDouble(); // uniform(0,1]
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                
                referenceArray[j] = (int)(lowMean + stdDev * randStdNormal);
            }
            
            int[] referenceArray2 = new int[thisN];
            for (int j = 0; j < thisN; j++) {
                double u1 = 1.0 - rand.NextDouble(); // uniform(0,1]
                double u2 = 1.0 - rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                
                referenceArray2[j] = (int)(highMean + stdDev * randStdNormal);
            }

            referenceArray = referenceArray.Concat(referenceArray2).ToArray();

            int[] array = (int[])referenceArray.Clone();
            YamSorter.Sort(array);
            Array.Sort(referenceArray);
            Assert.Equal(referenceArray, array);
        }
    }

    [Fact]
    public void SemiSequential()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            int[] referenceArray = Enumerable.Range(1, ARRAY_SIZE).ToArray();
            for (int j = 0; j < ARRAY_SIZE/10; j++)
            {
                referenceArray[rand.Next(0, ARRAY_SIZE - 1)] = rand.Next(1, ARRAY_SIZE);
            }
            int[] array = (int[])referenceArray.Clone();
            YamSorter.Sort(array);
            Array.Sort(referenceArray);
            Assert.Equal(referenceArray, array);
        }
    }

    [Fact]
    public void SemiReverseSequential()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            int[] referenceArray = Enumerable.Range(1, ARRAY_SIZE).Reverse().ToArray();
            for (int j = 0; j < ARRAY_SIZE/10; j++)
            {
                referenceArray[rand.Next(0, ARRAY_SIZE - 1)] = rand.Next(1, ARRAY_SIZE);
            }
            int[] array = (int[])referenceArray.Clone();
            YamSorter.Sort(array);
            Array.Sort(referenceArray);
            Assert.Equal(referenceArray, array);
        }
    }

    [Fact]
    public void CleanPipeOrgan()
    {
        int[] referenceArray = new int[ARRAY_SIZE];
        int peaks = 10;
        for (int j = 0; j < (peaks / 2); j++)
        {
            referenceArray = referenceArray.Concat(Enumerable.Range(1, ARRAY_SIZE/peaks).ToArray()).ToArray();
            referenceArray = referenceArray.Concat(Enumerable.Range(1, ARRAY_SIZE/peaks).Reverse().ToArray()).ToArray();
        }
        int[] array = (int[])referenceArray.Clone();
        YamSorter.Sort(array);
        Array.Sort(referenceArray);
        Assert.Equal(referenceArray, array);
    }

    [Fact]
    public void SemiPipeOrgan()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            int[] referenceArray = new int[ARRAY_SIZE];
            int peaks = 10;
            for (int j = 0; j < (peaks / 2); j++)
            {
                referenceArray = referenceArray.Concat(Enumerable.Range(1, ARRAY_SIZE/peaks).ToArray()).ToArray();
                referenceArray = referenceArray.Concat(Enumerable.Range(1, ARRAY_SIZE/peaks).Reverse().ToArray()).ToArray();
            }
            for (int j = 0; j < ARRAY_SIZE/10; j++)
            {
                referenceArray[rand.Next(0, ARRAY_SIZE - 1)] = rand.Next(1, ARRAY_SIZE);
            }
            int[] array = (int[])referenceArray.Clone();
            YamSorter.Sort(array);
            Array.Sort(referenceArray);
            Assert.Equal(referenceArray, array);
        }
    }

    [Fact]
    public void Binary()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            int[] referenceArray = new int[ARRAY_SIZE];
            for (int j = 0; j < ARRAY_SIZE; j++)
            {
                referenceArray[i] = rand.Next(0, 2);
            }
            int[] array = (int[])referenceArray.Clone();
            YamSorter.Sort(array);
            Array.Sort(referenceArray);
            Assert.Equal(referenceArray, array);
        }
    }
}


public class StabilityTests
{
    const int ARRAY_SIZE = 500000;
    const int LOOP_COUNT = 10;

    [Fact]
    public void NarrowValueRange()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            StableInt[] referenceArray = new StableInt[ARRAY_SIZE];
            Random rand = new(DateTime.UtcNow.Millisecond);
            for (int j = 0; j < ARRAY_SIZE; j++)
            {
                referenceArray[j] = new StableInt(rand.Next(1, ARRAY_SIZE / 500), j);
            }
            StableInt[] array = (StableInt[])referenceArray.Clone();
            YamSorter.Sort(array);
            Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
        }
    }

    [Fact]
    public void WideValueRange()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            StableInt[] referenceArray = new StableInt[ARRAY_SIZE];
            Random rand = new(DateTime.UtcNow.Millisecond);
            for (int j = 0; j < ARRAY_SIZE; j++)
            {
                referenceArray[j] = new StableInt(rand.Next(1, ARRAY_SIZE / 2), j);
            }
            StableInt[] array = (StableInt[])referenceArray.Clone();
            YamSorter.Sort(array);
            Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
        }
    }

    [Fact]
    public void Binary()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            StableInt[] referenceArray = new StableInt[ARRAY_SIZE];
            Random rand = new(DateTime.UtcNow.Millisecond);
            for (int j = 0; j < ARRAY_SIZE; j++)
            {
                referenceArray[j] = new StableInt(rand.Next(0, 1), j);
            }
            StableInt[] array = (StableInt[])referenceArray.Clone();
            YamSorter.Sort(array);
            Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
        }
    }
}


public class CustomComparerTests
{
    const int ARRAY_SIZE = 500000;
    const int LOOP_COUNT = 10;

    [Fact]
    public void CustomComparer()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            StableInt[] referenceArray = new StableInt[ARRAY_SIZE];
            Random rand = new(DateTime.UtcNow.Millisecond);
            for (int j = 0; j < ARRAY_SIZE; j++)
            {
                referenceArray[j] = new StableInt(rand.Next(1, ARRAY_SIZE / 10), j);
            }
            StableInt[] array = (StableInt[])referenceArray.Clone();
            YamSorter.Sort(array, new StableIntComparer());
            Assert.Equal(referenceArray.OrderBy(x => x.SortVal), array);
        }
    }

    private class StableIntComparer : IComparer<StableInt>
    {
        public int Compare(StableInt x, StableInt y)
        {
            return x.SortVal.CompareTo(y.SortVal);
        }
    }
}


public class IListTests
{
    const int ARRAY_SIZE = 500000;
    const int LOOP_COUNT = 10;
    Random rand = new(DateTime.UtcNow.Millisecond);

    [Fact]
    public void IListRandom()
    {
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            Random rand = new(DateTime.UtcNow.Millisecond);
            CustomIList<int> referenceArray = new();
            CustomIList<int> array = new();
            for (int j = 0; j < ARRAY_SIZE; j++)
            {
                int val = rand.Next(1, ARRAY_SIZE);
                referenceArray.Add(val);
                array.Add(val);
            }
            YamSorter.Sort(array);
            Assert.Equal(referenceArray.OrderBy(x => x), array);
        }
    }

    [Fact]
    public void IListSequential()
    {
        CustomIList<int> referenceArray = new();
        CustomIList<int> array = new();
        for (int j = 0; j < ARRAY_SIZE; j++)
        {
            int val = j + 1;
            referenceArray.Add(val);
            array.Add(val);
        }
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x), array);
    }


    [Fact]
    public void IListReverseSequential()
    {
        CustomIList<int> referenceArray = new();
        CustomIList<int> array = new();
        for (int j = 0; j < ARRAY_SIZE; j++)
        {
            int val = ARRAY_SIZE - j;
            referenceArray.Add(val);
            array.Add(val);
        }
        YamSorter.Sort(array);
        Assert.Equal(referenceArray.OrderBy(x => x), array);
    }
}