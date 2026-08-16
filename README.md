## Overview
YamSort is a stable sorting algorithm written in C# with optimizations for sorted and semi-sorted data

YamSort is compatible with .NET Standard 2.1 and works with all comparable data types (all types that implement `IComparable<T>`). It has a competitive performance profile with the unstable built-in `Array.Sort()` while being faster than the stable LINQ `OrderBy()` method in almost all situations.

YamSort accepts `T[]`, `Span<T>`, `List<T>`, and `IList<T>` data structures.  It has the option to sort those objects in place or return a new object.

## Performance Analysis

#### Time Complexity
- O(n) - Best case (when data is already sorted)
- O(n log n) - Average and worst case

#### Memory Complexity
- O(n) - Typically uses a single n/2-sized buffer (see note)
- GC Impact - Sources the buffer from ArrayPool, resulting in 0 heap allocations

NOTE: Creates and additional n-sized buffer when sorting `IList<T>` types, for a total memory use of 1.5n; in .NET 6+, sorting `List<T>` specifically does not require this extra buffer

#### Benchmark Highlights

```
1,000,000 Sequential Integers:
YamSort   |   .66 ms
ArraySort |  4.79 ms
OrderBy   | 11.62 ms

1,000,000 Random Integers:
YamSort   | 58.08 ms
ArraySort | 47.24 ms
OrderBy   | 75.97 ms

1,000,000 Near-Sequential Integers:
YamSort   | 13.73 ms
ArraySort | 20.62 ms
OrderBy   | 40.03 ms

Real-World Windows Log File With 109,546 Lines:
YamSort   | 38.63 ms
ArraySort | 66.98 ms
OrderBy   | 59.79 ms
```

## Usage

First, either import YamSort from NuGet or download `YamSort.cs` from GitHub and copy it into your project.

To use YamSort, use the `YamSort` namespace and call `YamSorter.Sort()` on your target array or list. A separate `YamSorter.SortReturning()` method is available to return a new sorted collection without modifying the original.

*Example:*
```
using YamSort;

public class YamSortDemo
{
    public void ExecuteYamSort()
    {
        int[] inputArray = [4, 1, 3, 2];
        YamSorter.Sort(inputArray);
        // inputArray now equals [1, 2, 3, 4]

        List<int> inputList = [4, 1, 3, 2];
        YamSorter.Sort(inputList);
        // inputList now equals [1, 2, 3, 4]
    }

    public void ExecuteYamSortReturning()
    {
        int[] referenceArray = [4, 1, 3, 2];
        int[] newArray = YamSorter.SortReturning(referenceArray);
        // referenceArray still equals [4, 1, 3, 2]
        // newArray equals [1, 2, 3, 4]

        CustomIList<int> referenceList = [4, 1, 3, 2];
        CustomIList<int> newList = (CustomIList<int>)YamSorter.SortReturning(referenceList);
        // referenceList still equals [4, 1, 3, 2]
        // newList equals [1, 2, 3, 4]
    }
}
```

## More Information and Benchmarks
Algorithm details and a large suite of benchmarks comparing YamSort to `Array.Sort()` and `OrderBy()` is available [here.](https://garygende.com/2026/08/yet-another-merge-sort/)