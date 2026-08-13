using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace YamSort
{

public static partial class YamSorter
{
    const int GALLOP_THRESHOLD = 5;
    const int SEQ_DEFAULT = 4;
    const int SEQ_MAX = 8;
    const int SEQ_MIN = 0;
    static readonly int[] numSeqValues = new int[] {10, 26, 40, 40, 40, 40, 40, 48, 60};
    static readonly int[] compSeqValues = new int[] {3, 6, 10, 10, 10, 10, 10, 15, 24};


    // PUBLIC INTERFACE//

    public static void Sort<T>(T[] arr) where T : IComparable<T>
    {
        SortSetup(arr.AsSpan());
    }

    public static T[] SortReturning<T>(T[] arr) where T : IComparable<T>
    {
        T[] newArr = (T[])arr.Clone();
        SortSetup(newArr.AsSpan());
        return newArr;
    }

#if NET6_0_OR_GREATER
    public static void Sort<T>(List<T> list) where T : IComparable<T>
    {
        SortSetup(CollectionsMarshal.AsSpan(list));
    }

    public static List<T> SortReturning<T>(List<T> list) where T : IComparable<T>
    {
        List<T> newList = new List<T>(list);
        SortSetup(CollectionsMarshal.AsSpan(newList));
        return newList;
    }
#endif


    // PRIVATE IMPLEMENTATION //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortSetup<T>(Span<T> span) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        if (typeof(T) == typeof(float) || typeof(T) == typeof(double)
#if NET6_0_OR_GREATER
            || typeof(T) == typeof(Half)
#endif
            )
        {
            int nanCount = MoveNansToFront(span);
            span = span[nanCount..];
        }

        SortState<T> state = new(false, SEQ_DEFAULT, Comparer<T>.Default, false);

        if (span.Length <= state.seqValues[state.seqScore])
        {
            InsertionSort(span, state.comparer);
            return;
        }

        T[] qBuffer = ArrayPool<T>.Shared.Rent((span.Length >> 1) + 2);

        try
        {
            ExecuteSort(span, qBuffer, ref state);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(qBuffer, clearArray: true);
        }
    }

    private static void ExecuteSort<T>(Span<T> span, Span<T> qBuffer, ref SortState<T> state) where T : IComparable<T>
    {
        int mid = (span.Length + 1) >> 1;  // div by 2, round up
        
        if (span.Length <= state.seqValues[state.seqScore])
        {
            InsertionSort(span, state.comparer);
            return;
        }

        ExecuteSort(span[..mid], qBuffer, ref state);
        ExecuteSort(span[mid..], qBuffer, ref state);

        if (state.reverseSortFound)
        {
            if (LessThan(ref span[^1], ref span[0], state.comparer))
            {
                span[..mid].CopyTo(qBuffer);
                span[mid..].CopyTo(span);
                qBuffer[..mid].CopyTo(span[(span.Length - mid)..]);
                if (state.seqScore > SEQ_MIN) state.seqScore--;
                return;
            }   
        }
        if (LessThanOrEqual(ref span[mid - 1], ref span[mid], state.comparer)) return;

        if (state.seqScore == SEQ_MAX || state.seqScore == SEQ_MIN)
            GallopMerge(span, mid - 1, qBuffer, ref state);
        else
            SimpleMerge(span, mid - 1, qBuffer, ref state);
    }


    // SORT ALGORITHMS //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InsertionSort<T, TComp>(Span<T> span, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        int length = span.Length;

        for (int i = 1; i < length; i++)
        {
            T key = span[i];
            int j = i - 1;
            while (j >= 0 && GreaterThan(ref span[j], ref key, comparer))
            {
                span[j + 1] = span[j];
                j--;
            }
            span[j + 1] = key;
        }
    }

    private static void SimpleMerge<T>(Span<T> span, int mid, Span<T> qBuffer, ref SortState<T> state) where T : IComparable<T>
    {
        int leftIdx = mid;
        int rightIdx = span.Length - 1;
        SpanQueue<T> bufferQ = new(qBuffer);

        // Check for sequential data at the beginning that is already in place and skip it
        while (GreaterThan(ref span[rightIdx], ref span[leftIdx], state.comparer))
            rightIdx--;

        // Copy the rest of the right side to the buffer
        bufferQ.Enqueue(span[(mid + 1)..(rightIdx + 1)]);

        // Fill the right side with values from the left side and the buffer
        while (rightIdx > mid)
        {
            if (!LessThanOrEqual(ref span[leftIdx], ref qBuffer[bufferQ.head], state.comparer))
                span[rightIdx--] = span[leftIdx--];
            else
                span[rightIdx--] = bufferQ.Dequeue();
        }

        // Update the sequence tracker based on how many items were in the buffer relative to the length of the span
        UpdateScore(bufferQ.count, span.Length, ref state);

        // Early exit for reverse sequential
        if (leftIdx < 0)
        {
            bufferQ.Dequeue(bufferQ.count).CopyTo(span);
            state.reverseSortFound = true;
            return;
        }

        // Fill the left side with the remaining values
        while (true)
        {
            if (GreaterThan(ref span[leftIdx], ref qBuffer[bufferQ.head], state.comparer))
            {
                span[rightIdx--] = span[leftIdx--];
                if (leftIdx < 0)
                {
                    bufferQ.Dequeue(bufferQ.count).CopyTo(span);
                    break;
                }
            }
            else
                span[rightIdx--] = bufferQ.Dequeue();
                if (bufferQ.count == 0)
                    break;
        }
    }

    private static void GallopMerge<T>(Span<T> span, int mid, Span<T> qBuffer, ref SortState<T> state) where T : IComparable<T>
    {
        int gallopCounter = 0;
        bool lastWasLeft = true;
        int leftIdx = mid;
        int rightIdx = span.Length - 1;
        SpanQueue<T> bufferQ = new(qBuffer);

        // Check for sequential data at the beginning that is already in place and skip it
        if (GreaterThan(ref span[rightIdx], ref span[leftIdx], state.comparer))
            rightIdx -= FindTargetInclusive(span, rightIdx, mid + 1, span[leftIdx], state.comparer);

        // Copy the rest of the right side to the buffer
        bufferQ.Enqueue(span[(mid + 1)..(rightIdx + 1)]);

        // Fill the right side with values from the left side and the buffer
        while (rightIdx > mid)
        {
            bool useBuffer = LessThanOrEqual(ref span[leftIdx], ref qBuffer[bufferQ.head], state.comparer);

            if (!useBuffer)
            {
                if (gallopCounter >= GALLOP_THRESHOLD) // Do Gallop
                {
                    int start = leftIdx - (rightIdx - mid - 1);
                    int numToCopy = FindTargetExclusive(span, leftIdx, start, bufferQ.Peek(), state.comparer);
                    span[(leftIdx - numToCopy + 1)..(leftIdx + 1)].CopyTo(span[(rightIdx - numToCopy + 1)..]);
                    
                    leftIdx -= numToCopy;
                    rightIdx -= numToCopy;
                    gallopCounter = 0;
                }
                else // No Gallop
                {
                    span[rightIdx--] = span[leftIdx--];

                    if (lastWasLeft)
                        gallopCounter++;
                    else
                    {
                        gallopCounter = 0;
                        lastWasLeft = true;
                    }
                }
            }
            else // Get data from buffer
            {
                if (gallopCounter >= GALLOP_THRESHOLD) // Do Gallop
                {
                    int start = bufferQ.head + 1 - (rightIdx - mid);
                    int numToCopy = FindTargetInclusive(qBuffer, bufferQ.head, start, span[leftIdx], state.comparer);
                    bufferQ.Dequeue(numToCopy).CopyTo(span[(rightIdx - numToCopy + 1)..]);

                    rightIdx -= numToCopy;
                    gallopCounter = 0;
                }
                else // No Gallop
                {
                    span[rightIdx--] = bufferQ.Dequeue();

                    if (!lastWasLeft)
                        gallopCounter++;
                    else
                    {
                        gallopCounter = 0;
                        lastWasLeft = false;
                    }
                }
            }
        }

        // Update the sequence tracker based on how many items were in the buffer realtive to the length of the span
        UpdateScore(bufferQ.count, span.Length, ref state);

        // Early exit for reverse sequential
        if (leftIdx < 0)
        {
            bufferQ.Dequeue(bufferQ.count).CopyTo(span);
            state.reverseSortFound = true;
            return;
        }

        // Fill the left side with the remaining values
        while (bufferQ.count > 0)
        {
            bool useBuffer = LessThanOrEqual(ref span[leftIdx], ref qBuffer[bufferQ.head], state.comparer);

            if (!useBuffer)
            {
                if (gallopCounter >= GALLOP_THRESHOLD) // Do Gallop
                {
                    int numToCopy = FindTargetExclusive(span, leftIdx, 0, bufferQ.Peek(), state.comparer);
                    span[(leftIdx - numToCopy + 1)..(leftIdx + 1)].CopyTo(span[(rightIdx - numToCopy + 1)..]);
                    
                    leftIdx -= numToCopy;
                    rightIdx -= numToCopy;
                    gallopCounter = 0;
                }
                else // No Gallop
                {
                    span[rightIdx--] = span[leftIdx--];

                    if (lastWasLeft)
                        gallopCounter++;
                    else
                    {
                        gallopCounter = 0;
                        lastWasLeft = true;
                    }
                }

                if (leftIdx < 0)
                {
                    bufferQ.Dequeue(bufferQ.count).CopyTo(span);
                    break;
                }
            }
            else // Get data from buffer
            {
                if (gallopCounter >= GALLOP_THRESHOLD) // Do Gallop
                {
                    int start = bufferQ.tail + 1;
                    int numToCopy = FindTargetInclusive(qBuffer, bufferQ.head, start, span[leftIdx], state.comparer);
                    bufferQ.Dequeue(numToCopy).CopyTo(span[(rightIdx - numToCopy + 1)..]);

                    rightIdx -= numToCopy;
                    gallopCounter = 0;
                }
                else // No Gallop
                {
                    span[rightIdx--] = bufferQ.Dequeue();

                    if (!lastWasLeft)
                        gallopCounter++;
                    else
                    {
                        gallopCounter = 0;
                        lastWasLeft = false;
                    }
                }
            }
        }
    }


    // SUPPORT METHODS AND DATA STRUCTURES //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindTargetExclusive<T, TComp>(Span<T> span, int index, int lowerBound, T targetVal, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        int count = 0;
        int jump = 1;

        // Find overshoot index
        do
        {
            count += jump;
            jump <<= 1; 
        }
        while (index - count >= lowerBound && GreaterThan(ref span[index - count], ref targetVal, comparer));

        if (index - count < lowerBound)
            count = index - lowerBound + 1;

        // Narrow down with binary search
        int lowIdx = index - count + 1;
        int highIdx = index;
        int comp = 0;
        while (lowIdx <= highIdx)
        {
            int midIdx = lowIdx + ((highIdx - lowIdx) >> 1);
            comp = comparer.Compare(span[midIdx], targetVal);
            if (comp > 0)
                highIdx = midIdx - 1;
            else
                lowIdx = midIdx + 1;
        }

        // Check for duplicates, be exclusive for stability
        while (comp == 0)
        {
            if (++highIdx >= index)
            {
                break;
            }
            comp = comparer.Compare(span[highIdx], targetVal);
        }

        return index - highIdx;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindTargetInclusive<T, TComp>(Span<T> span, int index, int lowerBound, T targetVal, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        int count = 0;
        int jump = 1;

        // Find overshoot index
        do
        {
            count += jump;
            jump <<= 1; 
        }
        while (index - count >= lowerBound && GreaterThan(ref span[index - count], ref targetVal, comparer));

        if (index - count < lowerBound)
            count = index - lowerBound + 1;

        // Narrow down with binary search
        int lowIdx = index - count + 1;
        int highIdx = index;
        int comp = 0;
        while (lowIdx <= highIdx)
        {
            int midIdx = lowIdx + ((highIdx - lowIdx) >> 1);
            comp = comparer.Compare(span[midIdx], targetVal);
            if (comp > 0)
                highIdx = midIdx - 1;
            else
                lowIdx = midIdx + 1;
        }

        // Check for duplicates, be inclusive for stability
        while (comp == 0)
        {
            if (--highIdx < lowerBound)
            {
                break;
            }
            comp = comparer.Compare(span[highIdx], targetVal);
        }

        return index - highIdx;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdateScore<T>(int count, int length, ref SortState<T> state) where T : IComparable<T>
    {
        int max = length >> 1;
        int threshold = max >> 2;
        if (count < threshold)  // sequential(ish) data detected
        {
            if (state.seqScore < SEQ_MAX) state.seqScore++;
        }
        else if (count > (max - threshold) && state.seqScore > SEQ_MIN)  // reverse sequential(ish) data detected
            state.seqScore--;
    }


    private ref struct SortState<T> where T : IComparable<T>
    {
        public IComparer<T> comparer;
        public bool reverseSortFound;
        public int seqScore;
        public int[] seqValues;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SortState(bool reverseSortFound, int seqScore, IComparer<T> comparer, bool customComparer)
        {
            this.comparer = comparer;
            this.reverseSortFound = reverseSortFound;
            this.seqScore = seqScore;
            if (customComparer)
                seqValues = compSeqValues;
            else if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte) || 
                    typeof(T) == typeof(ushort) || typeof(T) == typeof(short) || 
                    typeof(T) == typeof(uint) || typeof(T) == typeof(int) || 
                    typeof(T) == typeof(ulong) || typeof(T) == typeof(long) || 
                    typeof(T) == typeof(nuint) || typeof(T) == typeof(nint) ||
                    typeof(T) == typeof(float) || typeof(T) == typeof(double)
#if NET6_0_OR_GREATER
                    || typeof(T) == typeof(Half)
#endif
                    )
                seqValues = numSeqValues;
            else
                seqValues = compSeqValues;
        }
    }


    private ref struct SpanQueue<T>
    {
        public readonly Span<T> buffer;
        public int head;
        public int tail;
        public int count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanQueue(Span<T> existingSpan)
        {
            buffer = existingSpan;
            head = buffer.Length - 1;
            tail = buffer.Length - 1;
            count = 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T item)
        {
            buffer[tail--] = item;
            count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(ReadOnlySpan<T> items)
        {
            items.CopyTo(buffer[(tail - items.Length + 1)..]);
            tail -= items.Length;
            count += items.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Dequeue()
        {
            count--;
            return buffer[head--]; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> Dequeue(int number)
        {
            ReadOnlySpan<T> result = buffer.Slice(head - number + 1, number);
            head -= number;
            count -= number;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Drop(int number)
        {
            head -= number;
            count -= number;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Drop()
        {
            head--;
            count--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T Peek()
        {
            return buffer[head];
        }
    }


    // All below are copied or adapted from .NET source code (ArraySortHelper.cs)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GreaterThan<T, TComp>(ref T a, ref T b, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {

        if (typeof(T) == typeof(byte)) return (byte)(object)a > (byte)(object)b;
        if (typeof(T) == typeof(sbyte)) return (sbyte)(object)a > (sbyte)(object)b;
        if (typeof(T) == typeof(ushort)) return (ushort)(object)a > (ushort)(object)b;
        if (typeof(T) == typeof(short)) return (short)(object)a > (short)(object)b;
        if (typeof(T) == typeof(uint)) return (uint)(object)a > (uint)(object)b;
        if (typeof(T) == typeof(int)) return (int)(object)a > (int)(object)b;
        if (typeof(T) == typeof(ulong)) return (ulong)(object)a > (ulong)(object)b;
        if (typeof(T) == typeof(long)) return (long)(object)a > (long)(object)b;
        if (typeof(T) == typeof(nuint)) return (nuint)(object)a > (nuint)(object)b;
        if (typeof(T) == typeof(nint)) return (nint)(object)a > (nint)(object)b;
        if (typeof(T) == typeof(float)) return (float)(object)a > (float)(object)b;
        if (typeof(T) == typeof(double)) return (double)(object)a > (double)(object)b;
#if NET6_0_OR_GREATER
        if (typeof(T) == typeof(Half)) return (Half)(object)a > (Half)(object)b;
#endif
        return comparer.Compare(a, b) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool LessThan<T, TComp>(ref T a, ref T b, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        if (typeof(T) == typeof(byte)) return (byte)(object)a < (byte)(object)b;
        if (typeof(T) == typeof(sbyte)) return (sbyte)(object)a < (sbyte)(object)b;
        if (typeof(T) == typeof(ushort)) return (ushort)(object)a < (ushort)(object)b;
        if (typeof(T) == typeof(short)) return (short)(object)a < (short)(object)b;
        if (typeof(T) == typeof(uint)) return (uint)(object)a < (uint)(object)b;
        if (typeof(T) == typeof(int)) return (int)(object)a < (int)(object)b;
        if (typeof(T) == typeof(ulong)) return (ulong)(object)a < (ulong)(object)b;
        if (typeof(T) == typeof(long)) return (long)(object)a < (long)(object)b;
        if (typeof(T) == typeof(nuint)) return (nuint)(object)a < (nuint)(object)b;
        if (typeof(T) == typeof(nint)) return (nint)(object)a < (nint)(object)b;
        if (typeof(T) == typeof(float)) return (float)(object)a < (float)(object)b;
        if (typeof(T) == typeof(double)) return (double)(object)a < (double)(object)b;
#if NET6_0_OR_GREATER
        if (typeof(T) == typeof(Half)) return (Half)(object)a < (Half)(object)b;
#endif
        return comparer.Compare(a, b) < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool LessThanOrEqual<T, TComp>(ref T a, ref T b, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        if (typeof(T) == typeof(byte)) return (byte)(object)a <= (byte)(object)b;
        if (typeof(T) == typeof(sbyte)) return (sbyte)(object)a <= (sbyte)(object)b;
        if (typeof(T) == typeof(ushort)) return (ushort)(object)a <= (ushort)(object)b;
        if (typeof(T) == typeof(short)) return (short)(object)a <= (short)(object)b;
        if (typeof(T) == typeof(uint)) return (uint)(object)a <= (uint)(object)b;
        if (typeof(T) == typeof(int)) return (int)(object)a <= (int)(object)b;
        if (typeof(T) == typeof(ulong)) return (ulong)(object)a <= (ulong)(object)b;
        if (typeof(T) == typeof(long)) return (long)(object)a <= (long)(object)b;
        if (typeof(T) == typeof(nuint)) return (nuint)(object)a <= (nuint)(object)b;
        if (typeof(T) == typeof(nint)) return (nint)(object)a <= (nint)(object)b;
        if (typeof(T) == typeof(float)) return (float)(object)a <= (float)(object)b;
        if (typeof(T) == typeof(double)) return (double)(object)a <= (double)(object)b;
#if NET6_0_OR_GREATER
        if (typeof(T) == typeof(Half)) return (Half)(object)a <= (Half)(object)b;
#endif
        return comparer.Compare(a, b) <= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MoveNansToFront<T>(Span<T> span) where T : notnull
    {
        int left = 0;

        for (int i = 0; i < span.Length; i++)
        {
            if ((typeof(T) == typeof(double) && double.IsNaN((double)(object)span[i])) ||
                (typeof(T) == typeof(float) && float.IsNaN((float)(object)span[i]))
#if NET6_0_OR_GREATER
                || (typeof(T) == typeof(Half) && Half.IsNaN((Half)(object)span[i]))
#endif
                )
            {
                (span[i], span[left]) = (span[left], span[i]);
                left++;
            }
        }

        return left;
    }
}


// CUSTOM COMPARER VARIANTS //

public static partial class YamSorter
{

    // PUBLIC INTERFACE//
    public static void Sort<T>(T[] arr, IComparer<T> comparer) where T : IComparable<T>
    {
        SortSetupCC(arr.AsSpan(), comparer, true);
    }

    public static T[] SortReturning<T>(T[] arr, IComparer<T> comparer) where T : IComparable<T>
    {
        T[] newArr = (T[])arr.Clone();
        SortSetupCC(newArr.AsSpan(), comparer, true);
        return newArr;
    }

#if NET6_0_OR_GREATER
    public static void Sort<T>(List<T> list, IComparer<T> comparer) where T : IComparable<T>
    {
        SortSetupCC(CollectionsMarshal.AsSpan(list), comparer, true);
    }

    public static List<T> SortReturning<T>(List<T> list, IComparer<T> comparer) where T : IComparable<T>
    {
        List<T> newList = new List<T>(list);
        SortSetupCC(CollectionsMarshal.AsSpan(newList), comparer, true);
        return newList;
    }
#endif


    // PRIVATE IMPLEMENTATION //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortSetupCC<T, TComp>(Span<T> span, TComp comparer, bool customComparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        if (span.Length <= 1) return;

        if (typeof(T) == typeof(float) || typeof(T) == typeof(double)
#if NET6_0_OR_GREATER
            || typeof(T) == typeof(Half)
#endif
            )
        {
            int nanCount = MoveNansToFront(span);
            span = span[nanCount..];
        }

        SortState<T> state = new(false, SEQ_DEFAULT, comparer, customComparer);

        if (span.Length <= state.seqValues[state.seqScore])
        {
            InsertionSortCC(span, state.comparer);
            return;
        }

        T[] qBuffer = ArrayPool<T>.Shared.Rent((span.Length >> 1) + 2);

        try
        {
            ExecuteSortCC(span, qBuffer, ref state);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(qBuffer);
        }
    }

    private static void ExecuteSortCC<T>(Span<T> span, Span<T> qBuffer, ref SortState<T> state) where T : IComparable<T>
    {
        int mid = (span.Length + 1) >> 1;  // div by 2, round up
        
        if (span.Length <= state.seqValues[state.seqScore])
        {
            InsertionSortCC(span, state.comparer);
            return;
        }

        ExecuteSortCC(span[..mid], qBuffer, ref state);
        ExecuteSortCC(span[mid..], qBuffer, ref state);

        if (state.reverseSortFound)
        {
            if (LessThanCC(ref span[^1], ref span[0], state.comparer))
            {
                span[..mid].CopyTo(qBuffer);
                span[mid..].CopyTo(span);
                qBuffer[..mid].CopyTo(span[(span.Length - mid)..]);
                if (state.seqScore > SEQ_MIN) state.seqScore--;
                return;
            }   
        }
        if (LessThanOrEqualCC(ref span[mid - 1], ref span[mid], state.comparer)) return;

        if (state.seqScore == SEQ_MAX || state.seqScore == SEQ_MIN)
            GallopMergeCC(span, mid - 1, qBuffer, ref state);
        else
            SimpleMergeCC(span, mid - 1, qBuffer, ref state);
    }


    // SORT ALGORITHMS //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InsertionSortCC<T, TComp>(Span<T> span, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        int length = span.Length;

        for (int i = 1; i < length; i++)
        {
            T key = span[i];
            int j = i - 1;
            while (j >= 0 && GreaterThanCC(ref span[j], ref key, comparer))
            {
                span[j + 1] = span[j];
                j--;
            }
            span[j + 1] = key;
        }
    }

    private static void SimpleMergeCC<T>(Span<T> span, int mid, Span<T> qBuffer, ref SortState<T> state) where T : IComparable<T>
    {
        int leftIdx = mid;
        int rightIdx = span.Length - 1;
        SpanQueue<T> bufferQ = new(qBuffer);

        // Check for sequential data at the beginning that is already in place and skip it
        while (GreaterThanCC(ref span[rightIdx], ref span[leftIdx], state.comparer))
            rightIdx--;

        // Copy the rest of the right side to the buffer
        bufferQ.Enqueue(span[(mid + 1)..(rightIdx + 1)]);

        // Fill the right side with values from the left side and the buffer
        while (rightIdx > mid)
        {
            if (!LessThanOrEqualCC(ref span[leftIdx], ref qBuffer[bufferQ.head], state.comparer))
                span[rightIdx--] = span[leftIdx--];
            else
                span[rightIdx--] = bufferQ.Dequeue();
        }

        // Update the sequence tracker based on how many items were in the buffer realtive to the length of the span
        UpdateScore(bufferQ.count, span.Length, ref state);

        // Early exit for reverse sequential
        if (leftIdx < 0)
        {
            bufferQ.Dequeue(bufferQ.count).CopyTo(span);
            state.reverseSortFound = true;
            return;
        }

        // Fill the left side with the remaining values
        while (true)
        {
            if (GreaterThanCC(ref span[leftIdx], ref qBuffer[bufferQ.head], state.comparer))
            {
                span[rightIdx--] = span[leftIdx--];
                if (leftIdx < 0)
                {
                    bufferQ.Dequeue(bufferQ.count).CopyTo(span);
                    break;
                }
            }
            else
                span[rightIdx--] = bufferQ.Dequeue();
                if (bufferQ.count == 0)
                    break;
        }
    }

    private static void GallopMergeCC<T>(Span<T> span, int mid, Span<T> qBuffer, ref SortState<T> state) where T : IComparable<T>
    {
        int gallopCounter = 0;
        bool lastWasLeft = true;
        int leftIdx = mid;
        int rightIdx = span.Length - 1;
        SpanQueue<T> bufferQ = new(qBuffer);

        // Check for sequential data at the beginning that is already in place and skip it
        if (GreaterThanCC(ref span[rightIdx], ref span[leftIdx], state.comparer))
            rightIdx -= FindTargetInclusiveCC(span, rightIdx, mid + 1, span[leftIdx], state.comparer);

        // Copy the rest of the right side to the buffer
        bufferQ.Enqueue(span[(mid + 1)..(rightIdx + 1)]);

        // Fill the right side with values from the left side and the buffer
        while (rightIdx > mid)
        {
            bool useBuffer = LessThanOrEqualCC(ref span[leftIdx], ref qBuffer[bufferQ.head], state.comparer);

            if (!useBuffer)
            {
                if (gallopCounter >= GALLOP_THRESHOLD) // Do Gallop
                {
                    int start = leftIdx - (rightIdx - mid - 1);
                    int numToCopy = FindTargetExclusiveCC(span, leftIdx, start, bufferQ.Peek(), state.comparer);
                    span[(leftIdx - numToCopy + 1)..(leftIdx + 1)].CopyTo(span[(rightIdx - numToCopy + 1)..]);
                    
                    leftIdx -= numToCopy;
                    rightIdx -= numToCopy;
                    gallopCounter = 0;
                }
                else // No Gallop
                {
                    span[rightIdx--] = span[leftIdx--];

                    if (lastWasLeft)
                        gallopCounter++;
                    else
                    {
                        gallopCounter = 0;
                        lastWasLeft = true;
                    }
                }
            }
            else // Get data from buffer
            {
                if (gallopCounter >= GALLOP_THRESHOLD) // Do Gallop
                {
                    int start = bufferQ.head + 1 - (rightIdx - mid);
                    int numToCopy = FindTargetInclusiveCC(qBuffer, bufferQ.head, start, span[leftIdx], state.comparer);
                    bufferQ.Dequeue(numToCopy).CopyTo(span[(rightIdx - numToCopy + 1)..]);

                    rightIdx -= numToCopy;
                    gallopCounter = 0;
                }
                else // No Gallop
                {
                    span[rightIdx--] = bufferQ.Dequeue();

                    if (!lastWasLeft)
                        gallopCounter++;
                    else
                    {
                        gallopCounter = 0;
                        lastWasLeft = false;
                    }
                }
            }
        }

        // Update the sequence tracker based on how many items were in the buffer realtive to the length of the span
        UpdateScore(bufferQ.count, span.Length, ref state);

        // Early exit for reverse sequential
        if (leftIdx < 0)
        {
            bufferQ.Dequeue(bufferQ.count).CopyTo(span);
            state.reverseSortFound = true;
            return;
        }

        // Fill the left side with the remaining values
        while (bufferQ.count > 0)
        {
            bool useBuffer = LessThanOrEqualCC(ref span[leftIdx], ref qBuffer[bufferQ.head], state.comparer);

            if (!useBuffer)
            {
                if (gallopCounter >= GALLOP_THRESHOLD) // Do Gallop
                {
                    int numToCopy = FindTargetExclusiveCC(span, leftIdx, 0, bufferQ.Peek(), state.comparer);
                    span[(leftIdx - numToCopy + 1)..(leftIdx + 1)].CopyTo(span[(rightIdx - numToCopy + 1)..]);
                    
                    leftIdx -= numToCopy;
                    rightIdx -= numToCopy;
                    gallopCounter = 0;
                }
                else // No Gallop
                {
                    span[rightIdx--] = span[leftIdx--];

                    if (lastWasLeft)
                        gallopCounter++;
                    else
                    {
                        gallopCounter = 0;
                        lastWasLeft = true;
                    }
                }

                if (leftIdx < 0)
                {
                    bufferQ.Dequeue(bufferQ.count).CopyTo(span);
                    break;
                }
            }
            else // Get data from buffer
            {
                if (gallopCounter >= GALLOP_THRESHOLD) // Do Gallop
                {
                    int start = bufferQ.tail + 1;
                    int numToCopy = FindTargetInclusiveCC(qBuffer, bufferQ.head, start, span[leftIdx], state.comparer);
                    bufferQ.Dequeue(numToCopy).CopyTo(span[(rightIdx - numToCopy + 1)..]);

                    rightIdx -= numToCopy;
                    gallopCounter = 0;
                }
                else // No Gallop
                {
                    span[rightIdx--] = bufferQ.Dequeue();

                    if (!lastWasLeft)
                        gallopCounter++;
                    else
                    {
                        gallopCounter = 0;
                        lastWasLeft = false;
                    }
                }
            }
        }
    }


    // SUPPORT METHODS AND DATA STRUCTURES //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindTargetExclusiveCC<T, TComp>(Span<T> span, int index, int lowerBound, T targetVal, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        int count = 0;
        int jump = 1;

        // Find overshoot index
        do
        {
            count += jump;
            jump <<= 1; 
        }
        while (index - count >= lowerBound && GreaterThanCC(ref span[index - count], ref targetVal, comparer));

        if (index - count < lowerBound)
            count = index - lowerBound + 1;

        // Narrow down with binary search
        int lowIdx = index - count + 1;
        int highIdx = index;
        int comp = 0;
        while (lowIdx <= highIdx)
        {
            int midIdx = lowIdx + ((highIdx - lowIdx) >> 1);
            comp = comparer.Compare(span[midIdx], targetVal);
            if (comp > 0)
                highIdx = midIdx - 1;
            else
                lowIdx = midIdx + 1;
        }

        // Check for duplicates, be exclusive for stability
        while (comp == 0)
        {
            if (++highIdx >= index)
            {
                break;
            }
            comp = comparer.Compare(span[highIdx], targetVal);
        }

        return index - highIdx;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindTargetInclusiveCC<T, TComp>(Span<T> span, int index, int lowerBound, T targetVal, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        int count = 0;
        int jump = 1;

        // Find overshoot index
        do
        {
            count += jump;
            jump <<= 1; 
        }
        while (index - count >= lowerBound && GreaterThanCC(ref span[index - count], ref targetVal, comparer));

        if (index - count < lowerBound)
            count = index - lowerBound + 1;

        // Narrow down with binary search
        int lowIdx = index - count + 1;
        int highIdx = index;
        int comp = 0;
        while (lowIdx <= highIdx)
        {
            int midIdx = lowIdx + ((highIdx - lowIdx) >> 1);
            comp = comparer.Compare(span[midIdx], targetVal);
            if (comp > 0)
                highIdx = midIdx - 1;
            else
                lowIdx = midIdx + 1;
        }

        // Check for duplicates, be inclusive for stability
        while (comp == 0)
        {
            if (--highIdx < lowerBound)
            {
                break;
            }
            comp = comparer.Compare(span[highIdx], targetVal);
        }

        return index - highIdx;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GreaterThanCC<T, TComp>(ref T a, ref T b, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        return comparer.Compare(a, b) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool LessThanCC<T, TComp>(ref T a, ref T b, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        return comparer.Compare(a, b) < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool LessThanOrEqualCC<T, TComp>(ref T a, ref T b, TComp comparer)
        where T : IComparable<T> where TComp : IComparer<T>
    {
        return comparer.Compare(a, b) <= 0;
    }
}


}